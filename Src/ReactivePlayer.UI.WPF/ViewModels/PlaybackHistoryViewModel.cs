using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackHistoryViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngineAsync _audioPlayer;
        private readonly IReadLibraryService _readLibraryService;

        #endregion

        #region constructors

        public PlaybackHistoryViewModel(
            IAudioPlaybackEngineAsync audioPlaybackEngine,
            IReadLibraryService readLibraryService)
        {
            this._audioPlayer = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine)); // TODO: localize
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize

            // TODO: review operators order, e.g. where should .DisposeMany() be placed?
            // TODO: does this represent a subscription that should be disposed?
            //this._audioPlayer
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

        private ReadOnlyObservableCollection<PlaybackHistoryItemViewModel> _items;
        public ReadOnlyObservableCollection<PlaybackHistoryItemViewModel> Items => this._items;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        private CompositeDisposable _disposables = new CompositeDisposable();
        private object _disposingLock = new object();

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        public void Dispose()
        {
            try
            {
                lock (this._disposingLock)
                {
                    if (this._disposables != null && !this._disposables.IsDisposed)
                    {
                        this._disposables?.Dispose();
                        this._disposables = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Dispose)}: {ex.Message}");
                throw ex;
            }
        }

        #endregion
    }
}