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

    //#region IDisposable

    //// https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
    //private readonly CompositeDisposable _disposables = new CompositeDisposable();
    //private bool _isDisposed = false;

    //// use this in derived class
    //// protected override void Dispose(bool isDisposing)
    //// use this in non-derived class
    //protected virtual void Dispose(bool isDisposing)
    //{
    //    if (this._isDisposed)
    //        return;

    //    if (isDisposing)
    //    {
    //        // free managed resources here
    //        this._disposables.Dispose();
    //    }

    //    // free unmanaged resources (unmanaged objects) and override a finalizer below.
    //    // set large fields to null.

    //    this._isDisposed = true;
    //}

    //// remove if in derived class
    //public void Dispose()
    //{
    //    // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
    //    this.Dispose(true);
    //}

    //#endregion
    //}
}