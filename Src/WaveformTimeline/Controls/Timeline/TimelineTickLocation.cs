using System;
using WaveformTimeline.Commons;

namespace WaveformTimeline.Controls.Timeline
{
    public sealed class TimelineTickLocation
    {
        public TimelineTickLocation(TuneDuration tuneDuration, WaveformDimensions waveformDimensions)
        {
            this._tuneDuration = tuneDuration;
            this._waveformDimensions = waveformDimensions;
        }

        private readonly TuneDuration _tuneDuration;
        private readonly WaveformDimensions _waveformDimensions;

        public double LocationOnXAxis(TimeSpan sec) =>
            this._waveformDimensions.LeftMargin() + (sec.TotalSeconds - this._tuneDuration.StartingPoint()) / this._tuneDuration.Duration() * this._waveformDimensions.Width();

    }
}
