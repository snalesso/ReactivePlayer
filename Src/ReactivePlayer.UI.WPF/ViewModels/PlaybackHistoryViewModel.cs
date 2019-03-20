using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Playback.History;
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
            //this._audioPlayer = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine)); // TODO: localize
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize
            this._playbackHistory = playbackHistory ?? throw new ArgumentNullException(nameof(playbackHistory)); // TODO: localize

            var sorter = SortExpressionComparer<PlaybackHistoryEntryViewModel>.Descending(pheVM => pheVM.PlaybackEndedDateTime);
            //var sorter2 = new SortExpressionComparer<PlaybackHistoryEntryViewModel>();

            // TODO: review operators order, e.g. where should .DisposeMany() be placed?
            // TODO: does this represent a subscription that should be disposed?
            this._playbackHistory.Entries
                 .Connect()
                 .Transform(phe => new PlaybackHistoryEntryViewModel(phe))
                 //.DisposeMany()
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