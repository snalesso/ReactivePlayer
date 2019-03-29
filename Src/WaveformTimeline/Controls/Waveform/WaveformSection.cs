using System;
using WaveformTimeline.Commons;
using WaveformTimeline.Contracts;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Controls.Waveform
{
    public struct WaveformSection
    {
        public WaveformSection(TuneDuration tuneDuration, ITimedPlayback tune, int resolution)
        {
            this.Start = new Even((int)(new FiniteDouble(  // gotta be even, baby
                                    tuneDuration.StartingPoint() // this is number of seconds at which the area start
                                    / tune.TotalTime().TotalSeconds) // this is the total number of seconds - so the ratio is correct
                                .Value() * resolution));

            this.End = this.Start + (int)Math.Min(resolution, // how much are we going to cover? max is the number of points available <=> resolution
                      resolution / tuneDuration.Zoom); // what is this if resolution == 8000 and zoom == 2, then we cover r/2 == 4000? seems like!                
        }

        public int Start { get; }

        public int End { get; }

        public static bool operator ==(WaveformSection wf1, WaveformSection wf2)
        {
            return wf1.Equals(wf2);
        }

        public static bool operator !=(WaveformSection wf1, WaveformSection wf2)
        {
            return !(wf1 == wf2);
        }

        public override bool Equals(object obj)
        {
            return obj is WaveformSection section && this.Start == section.Start && this.End == section.End;
        }

        public bool Equals(WaveformSection other)
        {
            return this.Start == other.Start && this.End == other.End;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Start * 397) ^ this.End;
            }
        }
    }
}
