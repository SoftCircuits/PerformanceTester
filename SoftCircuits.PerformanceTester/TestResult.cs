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
    }
}
