using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Common.Extensions
{
    public class StopwatchBenchmarkResult
    {
        public StopwatchBenchmarkResult(IEnumerable<TimeSpan> times)
        {
            this.Avg = TimeSpan.FromTicks(Convert.ToInt64(times.Average(ts => ts.Ticks)));
            this.Min = times.Min();
            this.Max = times.Max();
        }

        public StopwatchBenchmarkResult(TimeSpan avg, TimeSpan min, TimeSpan max)
        {
            this.Avg = avg;
            this.Min = min;
            this.Max = max;
        }

        public TimeSpan Avg { get; }
        public TimeSpan Min { get; }
        public TimeSpan Max { get; }
    }
}