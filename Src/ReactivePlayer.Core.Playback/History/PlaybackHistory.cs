using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.Core.Playback.History
{
    public class PlaybackHistory : IDisposable
    {
        private readonly IAudioPlaybackEngineSync _audioPlaybackEngine;

        public PlaybackHistory(IAudioPlaybackEngineSync audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            this.Entries = this._audioPlaybackEngine.WhenTrackChanged
                .SkipLast(1)
                .Where(t => t != null)
                .ToObservableChangeSet(50)
                //.Replay(3)
                //.Select(track => new PlaybackHistoryEntry(track))
                .Transform(track => new PlaybackHistoryEntry(track))
                .DisposeMany()
                .AsObservableList()
                .DisposeWith(this._disposables);

            //this.Entries.CountChanged.Subscribe(c => Debug.WriteLine("items: " + c));
        }

        public IObservableList<PlaybackHistoryEntry> Entries { get; }

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