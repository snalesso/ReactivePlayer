using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Playback.History;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackHistoryViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly IAudioPlaybackEngine _audioPlayer;
        //private readonly IReadLibraryService _readLibraryService;
        private readonly PlaybackHistory _playbackHistory;

        #endregion

        #region constructors

        public PlaybackHistoryViewModel(
            PlaybackHistory playbackHistory
            //IAudioPlaybackEngine audioPlaybackEngine
            //, IReadLibraryService readLibraryService
            )
        {
            //this._audioPlayer = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._playbackHistory = playbackHistory ?? throw new ArgumentNullException(nameof(playbackHistory));

            var sorter = SortExpressionComparer<PlaybackHistoryEntryViewModel>.Descending(pheVM => pheVM.PlaybackEndedDateTime);
            //var sorter2 = new SortExpressionComparer<PlaybackHistoryEntryViewModel>();

            this._playbackHistory.Entries
                 .Connect()
                 .Transform(phe => new PlaybackHistoryEntryViewModel(phe))
                 .DisposeMany()
                 .Sort(
                    SortExpressionComparer<PlaybackHistoryEntryViewModel>.Descending(pheVM => pheVM.PlaybackEndedDateTime),
                    SortOptions.UseBinarySearch)
                 .Bind(out var boundEntries)
                 .Subscribe()
                 .DisposeWith(this._disposables);
            this.Entries = boundEntries;
            //    .WhenAudioSourceLocationChanged
            //    .ToObservableChangeSet(10)
            //    .Transform(location => new PlaybackHistoryItemViewModel(location))
            //    .Bind(out this._items)
            //    .DisposeMany()
            //    .Subscribe()
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<PlaybackHistoryEntryViewModel> _entries;
        public ReadOnlyObservableCollection<PlaybackHistoryEntryViewModel> Entries
        {
            get => this._entries;
            private set => this.RaiseAndSetIfChanged(ref this._entries, value);
        }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}