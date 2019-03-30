using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Services
{
    //public class WaveformTimelineSoundPlayer : ReactiveObject, WPFSoundVisualizationLib.IWaveformPlayer, IDisposable
    //{
    //    private readonly IAudioPlaybackEngine _audioPlaybackEngine;

    //    public WaveformTimelineSoundPlayer(IAudioPlaybackEngine audioPlaybackEngine)
    //    {
    //        this._audioPlaybackEngine = audioPlaybackEngine;

    //        this.WhenIsPlayingChanged = this._audioPlaybackEngine.WhenStatusChanged
    //            .Select(status => status == PlaybackStatus.Playing);

    //        this._isPlaying_OAPH = this._audioPlaybackEngine.WhenStatusChanged
    //            .Select(status => status == PlaybackStatus.Playing)
    //            .ToProperty(this, nameof(this.IsPlaying), deferSubscription: true)
    //            .DisposeWith(this._disposables);
    //    }

    //    private readonly ObservableAsPropertyHelper<bool> _isPlaying_OAPH;
    //    public bool IsPlaying => this._isPlaying_OAPH.Value;

    //    public IObservable<bool> WhenIsPlayingChanged { get; }

    //    public double ChannelPosition
    //    {
    //        get => throw new NotImplementedException();
    //        set => throw new NotImplementedException();
    //    }

    //    public double ChannelLength => throw new NotImplementedException();

    //    public float[] WaveformData => throw new NotImplementedException();

    //    public TimeSpan SelectionBegin
    //    {
    //        get => TimeSpan.Zero;
    //        set { }
    //    }

    //    public TimeSpan SelectionEnd
    //    {
    //        get => this._audioPlaybackEngine.Duration.GetValueOrDefault(TimeSpan.Zero);
    //        set { }
    //    }

    //    #region IDisposable Support

    //    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    //    private bool disposedValue = false; // To detect redundant calls

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!this.disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                this._disposables.Dispose();
    //            }

    //            // free unmanaged resources (unmanaged objects) and override a finalizer below.
    //            // set large fields to null.

    //            this.disposedValue = true;
    //        }
    //    }

    //    // This code added to correctly implement the disposable pattern.
    //    public void Dispose()
    //    {
    //        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //        this.Dispose(true);
    //    }
    //    #endregion
    //}
}