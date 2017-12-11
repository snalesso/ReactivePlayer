using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public class PlaybackQueue : IDisposable
    {
        #region constants & fields

        private const ushort HistoryMaxLength = 10;

        private readonly IPlaybackService _audioPlayer;
        //private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region ctor

        public PlaybackQueue(IPlaybackService audioPlayer)
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
                    var next = this._items.Items.FirstOrDefault();

                    if (next != null)
                    {
                        this._items.RemoveAt(0);
                        await this._audioPlayer.LoadAsync(next);
                        await this._audioPlayer.PlayAsync();
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

        private SourceList<Uri> _items = new SourceList<Uri>();
        public IObservableList<Uri> Items => this._items.AsObservableList();

        public IObservable<bool> IsEmpty => this.Items.Connect().IsEmpty();

        //public BehaviorSubject<Uri> _currentlyPlayingSubject = new BehaviorSubject<Uri>(null);
        //public IObservable<Uri> CurrentlyPlaying => this._audioPlayer.WhenTrackLocationChanged;

        #endregion

        #region methods

        public void Clear()
        {
            this._items.Clear();
        }

        private IObservableList<Uri> _playlist;
        public void SetPlaylist(IObservableList<Uri> playlist)
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

        // TODO: dequeu an IAudioSource which will carry its .Location
        public Uri Deqeue()
        {
            var head = this._items.Items.FirstOrDefault();

            this._items.RemoveAt(0);

            return head;
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