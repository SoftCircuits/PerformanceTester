// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.PerformanceTester
{
    public class TestResult
    {
        /// <summary>
        /// Returns the description of the test this result is for.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Returns the number of milliseconds this test took to run.
        /// </summary>
        public long Milliseconds { get; internal set; }

        /// <summary>
        /// Returns the relative performance of this test as a percent
        /// of the longest (slowest) test result.
        /// </summary>
        public int Percent { get; internal set; }

        /// <summary>
        /// Returns a string that shows the relative performance of this result.
        /// The string will include <paramref name="maxBarLength"/> characters
        /// with the <paramref name="c"/> character filling up the percent of
        /// the bar indicated by the <see cref="Percent"/> property.
        /// </summary>
        /// <param name="maxBarLength">The length of the bar in characters.</param>
        /// <param name="c">The character to use to</param>
        /// <returns>A string that shows the relative performance of this result.</returns>
        public string GetRelativePerformanceBar(int maxBarLength, char c = '*')
        {
            int count = (int)(Percent / 100.0 * maxBarLength);
            return new string(c, count) + new string(' ', maxBarLength - count);
        }
    }
}
