using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IDialogService _dialogService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IReadLibraryService _readLibraryService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            IDialogService dialogService,
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod)
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));
            this._editTrackTagsViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));

            var trackVMsFlow = this._readLibraryService
                .Tracks
                .Connect()
                .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                .DisposeMany() // TODO: put ALAP or ASAP?
                ;

            if (this.Filter != null)
            {
                trackVMsFlow = trackVMsFlow.Filter(this.Filter);
            }

            trackVMsFlow
                .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime))
                .Bind(out this._filteredSortedTrackViewModels)
                .Subscribe()
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
                    await this._audioPlaybackEngine.LoadAndPlayAsync(trackVM.Track)/*.ConfigureAwait(false)*/;
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel),
                    this._audioPlaybackEngine.WhenCanLoadChanged,
                    this._audioPlaybackEngine.WhenCanPlayChanged,
                    this._audioPlaybackEngine.WhenCanStopChanged,
                    (selectedTrackViewModel, canLoad, canPlay, canStop) => selectedTrackViewModel != null && (canLoad || canPlay || canStop)))
                .DisposeWith(this._disposables);

            this.PlayTrack.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);

            this.EditTrackTags = ReactiveCommand.Create(
                (TrackViewModel vm) =>
                {
                    this._dialogService.ShowDialog(this._editTrackTagsViewModelFactoryMethod(vm.Track));
                },
                this.WhenAny(x => x.SelectedTrackViewModel, x => x.Value != null)
                ).DisposeWith(this._disposables);
        }

        #endregion

        #region filtering

        protected abstract Func<TrackViewModel, bool> Filter { get; }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<TrackViewModel> _filteredSortedTrackViewModels;
        public ReadOnlyObservableCollection<TrackViewModel> FilteredSortedTrackViewModels
        {
            get => this._filteredSortedTrackViewModels;
            private set => this.RaiseAndSetIfChanged(ref this._filteredSortedTrackViewModels, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }

        public ReactiveCommand<TrackViewModel, Unit> EditTrackTags { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}