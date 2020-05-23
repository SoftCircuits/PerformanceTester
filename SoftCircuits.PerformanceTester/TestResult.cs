// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Returns a string that shows the performance of this result relative
        /// to the other results. <paramref name="maxBarLength"/> determines the
        /// total number of characters in the string. The <see cref="Percent"/>
        /// property determines the percent of those characters that are the
        /// character specified by <paramref name="c"/>. The remaining characters
        /// are spaces.
        /// </summary>
        /// <param name="maxBarLength">Total length of the string, in characters.</param>
        /// <param name="c">The character to use to render the bar.</param>
        /// <returns>A string that shows the performance of this result relative
        /// to the other results.</returns>
        public string GetRelativePerformanceBar(int maxBarLength, char c = '*')
        {
            int count = (int)(Percent / 100.0 * maxBarLength);
            return new string(c, count) + new string(' ', maxBarLength - count);
        }

        /// <summary>
        /// Sets the <see cref="Percent"/> property of each result to the percent
        /// relative to the slowest of all the results.
        /// </summary>
        /// <param name="results">The collection of <see cref="TestResult"/>s
        /// to modify.</param>
        public static void SetResultPercentages(IEnumerable<TestResult> results)
        {
            // Calculate result percentages (as a percent of slowest result)
            long maxMilliseconds = results.Select(r => r.Milliseconds).DefaultIfEmpty(0).Max();
            // Avoid divide by zero
            if (maxMilliseconds == 0)
            {
                foreach (TestResult result in results)
                    result.Percent = 0;
            }
            else
            {
                foreach (TestResult result in results)
                    result.Percent = Math.Min((int)((double)result.Milliseconds / maxMilliseconds * 100), 100);
            }
        }
    }
}
