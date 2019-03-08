namespace ReactivePlayer.Core.Playback
{
    // TODO: review use cases (KISS principle)
    public enum PlaybackStatus
    {
        None = 0,
        Loading,
        Loaded,
        Playing,
        Paused,
        ManuallyInterrupted,
        PlayedToEnd,
        Exploded
    }
}