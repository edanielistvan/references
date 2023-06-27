using System;
using System.Diagnostics;

namespace VisualSorting.Extensions
{
    public static class StopwatchExtensions
    {
        public static double ElapsedMicroseconds(this Stopwatch stopwatch)
        {
            if (stopwatch == null)
                throw new ArgumentException("Stopwatch passed cannot be null!");

            return 1e6 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
        }
    }
}
