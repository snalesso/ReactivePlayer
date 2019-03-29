using System;
using WaveformTimeline.Commons;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Controls.Timeline
{
    public sealed class EndRevealingTimeline : ITimelineMarkingStrategy
    {
        public EndRevealingTimeline(TuneDuration tuneDuration, TimeSpan firstMark): this(tuneDuration, firstMark, 0.8) {}

        public EndRevealingTimeline(TuneDuration tuneDuration, TimeSpan firstMark, ZeroToOne end)
        {
            this._firstMark = firstMark;
            var duration = tuneDuration.Duration();
            this._tickDuration = new TickDuration(duration);
            this._lastThirdMarkTs = TimeSpan.FromSeconds(duration * end);
            this._lastThirdDuration = new TickDuration(duration - this._lastThirdMarkTs.TotalSeconds);
        }

        private readonly TimeSpan _firstMark;
        private readonly TickDuration _lastThirdDuration;
        private readonly TimeSpan _lastThirdMarkTs;
        private readonly TickDuration _tickDuration;

        public bool AtMinorTick(TimeSpan sec)
        {
            return sec == this._firstMark ||
                   (sec < this._lastThirdMarkTs && Math.Abs(sec.Seconds % this._tickDuration.Minor) < 0.001) ||
                   (sec >= this._lastThirdMarkTs && Math.Abs(sec.Seconds % this._lastThirdDuration.Minor) < 0.001);
        }

        public bool AtMajorTick(TimeSpan sec)
        {
            return sec == this._firstMark ||
                   (sec < this._lastThirdMarkTs && Math.Abs(sec.Seconds % this._tickDuration.Major) < 0.001) ||
                   (sec >= this._lastThirdMarkTs && Math.Abs(sec.Seconds % this._lastThirdDuration.Major) < 0.001);
        }
    }
}