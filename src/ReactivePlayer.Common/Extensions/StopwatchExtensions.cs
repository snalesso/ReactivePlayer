using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Common.Extensions
{
    public static class StopwatchExtensions
    {
        public static StopwatchBenchmarkResult GetActionBenchmark(this Stopwatch stopwatch, Action action, uint repeatCount, bool collectBetweenExecutions = true)
        {
            if (repeatCount < 0)
                throw new ArgumentOutOfRangeException(nameof(repeatCount));

            IList<TimeSpan> times = new List<TimeSpan>();

            while (repeatCount-- > 0)
            {
                stopwatch.Restart();

                action();

                stopwatch.Stop();

                times.Add(stopwatch.Elapsed);

                if (collectBetweenExecutions)
                    GC.Collect();
            }

            return new StopwatchBenchmarkResult(times);
        }

        public static async Task<StopwatchBenchmarkResult> GetActionBenchmarkAsync(this Stopwatch stopwatch, Task asyncAction, int repeatCount, bool collectBetweenExecutions = true)
        {
            if (repeatCount < 0)
                throw new ArgumentOutOfRangeException(nameof(repeatCount));

            IList<TimeSpan> times = new List<TimeSpan>();

            while (repeatCount-- > 0)
            {
                stopwatch.Restart();

                await asyncAction;

                stopwatch.Stop();

                times.Add(stopwatch.Elapsed);

                if (collectBetweenExecutions)
                    GC.Collect();
            }

            return new StopwatchBenchmarkResult(times);
        }
    }
}