namespace ReactivePlayer.App
{
    // TODO: review use cases (KISS principle)
    public enum PlaybackStatus
    {
        NaturallyEnded = 0,
        Loading,
        Loaded,
        Playing,
        Paused,
        ManuallyStopped,
        Exploded
    }
}