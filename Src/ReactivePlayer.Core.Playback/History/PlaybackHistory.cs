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
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public PlaybackHistory(IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            this.Entries = this._audioPlaybackEngine.WhenTrackChanged
                .SkipLast(1)
                .Where(t => t!= null)
                .ToObservableChangeSet(50)
                //.Replay(3)
                //.Select(track => new PlaybackHistoryEntry(track))
                .Transform(track => new PlaybackHistoryEntry(track))
                .DisposeMany()
                .AsObservableList()
                .DisposeWith(this._disposables)
                ;

            //this.Entries.CountChanged.Subscribe(c => Debug.WriteLine("items: " + c));
        }

        public IObservableList<PlaybackHistoryEntry> Entries { get; }

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}