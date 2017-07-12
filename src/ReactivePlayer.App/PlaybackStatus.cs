namespace ReactivePlayer.App
{
    // TODO: review use cases (KISS principle)
    public enum PlaybackStatus
    {
        None,
        Playing,
        Paused,
        Buffering, // TODO: add support in observable in player implementation
        Stoppped,
        Ended,
        Errored
    }
}