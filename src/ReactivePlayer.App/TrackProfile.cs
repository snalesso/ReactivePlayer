using System;

namespace ReactivePlayer.App
{
    public sealed class TrackProfile
    {
        internal TrackProfile(Uri location)
        {
            this.Location = location;
        }

        public Uri Location { get; }
        public TimeSpan? Duration { get; internal set; }
        public Tags Tags { get; internal set; }
    }
}