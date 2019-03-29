using System;
using WaveformTimeline.Commons;

namespace WaveformTimeline.Controls.Timeline
{
    public sealed class ConstantTimeline : ITimelineMarkingStrategy
    {
        public ConstantTimeline(TuneDuration tuneDuration, TimeSpan firstMark)
        {
            this._firstMark = firstMark;
            this._tickDuration = new TickDuration(tuneDuration.Duration());
        }

        private readonly TimeSpan _firstMark;
        private readonly TickDuration _tickDuration;

        public bool AtMinorTick(TimeSpan sec)
        {
            return sec == this._firstMark || Math.Abs(sec.Seconds % this._tickDuration.Minor) < 0.001;
        }

        public bool AtMajorTick(TimeSpan sec)
        {
            return sec == this._firstMark || Math.Abs(sec.Seconds % this._tickDuration.Major) < 0.001;
        }
    }
}