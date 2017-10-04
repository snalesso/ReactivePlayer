namespace ReactivePlayer.Core.Application.Services.Playback
{
    // TODO: review use cases (KISS principle)
    public enum PlaybackStatus
    {
        None = 0,
        Loading,
        Loaded,
        Playing,
        Paused,
        Interrupted,
        Ended,
        Exploded
    }
}