// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.PerformanceTester
{
    public interface IPerformanceTest
    {
        /// <summary>
        /// Read-only property that returns a description of this test.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The method that performs the actual test. This is the
        /// method that will be timed.
        /// </summary>
        void Run(object data);
    }
}
