using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.Core.Playback.Queue
{
    public class PlaybackQueue : IDisposable
    {
        #region constants & fields

        private const ushort HistoryMaxLength = 10;

        private readonly IAudioPlaybackEngine _audioPlayer;
        //private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region ctor

        public PlaybackQueue(IAudioPlaybackEngine audioPlayer)
        {
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer)); // TODO: localize

            Observable
                .CombineLatest(
                    this._audioPlayer.WhenStatusChanged,
                    this._audioPlayer.WhenCanPlayChanged,
                    (status, canPlay) => status == PlaybackStatus.PlayedToEnd && canPlay == true)
                .Where(canPlayNext => canPlayNext == true)
                .Subscribe(async (s) =>
                {
                    var next = this._queueEntries.Items.FirstOrDefault();

                    if (next != null)
                    {
                        this._queueEntries.RemoveAt(0);

                        switch (next.LibraryEntry)
                        {
                            case Track nextTrack:
                                await this._audioPlayer.LoadAsync(nextTrack);
                                await this._audioPlayer.PlayAsync();
                                break;
                        }
                    }
                    else
                    {
                        // unsubscribe from playlist?
                    }
                })
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        //private ConcurrentQueue<Uri> _queue;
        private SourceList<PlaybackQueueEntry> _queueEntries = new SourceList<PlaybackQueueEntry>();
        public IObservableList<PlaybackQueueEntry> Entries => this._queueEntries;

        //public IObservable<bool> IsEmpty => this.Items.Connect().IsEmpty();

        //public BehaviorSubject<Uri> _currentlyPlayingSubject = new BehaviorSubject<Uri>(null);
        //public IObservable<Uri> CurrentlyPlaying => this._audioPlayer.WhenTrackLocationChanged;

        #endregion

        #region methods

        public void Clear()
        {
            this._queueEntries.Clear();
        }

        public void Enqueue(IEnumerable<PlaybackQueueEntry> trackLocations)
        {
            this._queueEntries.AddRange(trackLocations);
        }

        private IObservableList<PlaybackQueueEntry> _playlist;
        public void SetPlaylist(IObservableList<PlaybackQueueEntry> playlist)
        {
            this._playlist = playlist;

            // TODO: if the currently playing track is removed from the playlist from which its playback was started, ask if stop playback

            //return Task.CompletedTask;
        }

        //public void Enqueue(Uri trackLocation)
        //{
        //    this._items.Insert(this._items.Count, trackLocation);
        //}

        //public void Enqueue(IReadOnlyList<Uri> trackLocations)
        //{
        //    this._items.InsertRange(trackLocations, this._items.Count);
        //}

        // TODO: dequeue an IAudioSource which will carry its .Location
        public void Remove(PlaybackQueueEntry queueEntry)
        {
            //var head = this._queueItems.Items.FirstOrDefault();

            this._queueEntries.RemoveAt(0);

            //return head;
        }

        //private readonly BehaviorSubject<bool> _isShuffling;
        //public IObservable<bool> WhenIsShufflingChanged => this._isShuffling.AsObservable();

        //public void SetIsShuffling(bool isShuffling)
        //{
        //    this._isShuffling.OnNext(isShuffling);
        //}

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