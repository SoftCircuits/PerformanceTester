// Copyright (c) 2020-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.PerformanceTester
{
    public interface IPerformanceTest
    {
        /// <summary>
        /// Read-only property that returns a description of this test.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A method to perform any initialization. Time to perform this initialization
        /// is not included in the test results.
        /// </summary>
        /// <param name="data">Data passed to <see cref="PerformanceTester.Run(IPerformanceTest, int, object)"/></param>
        public void Initialize(object? data);

        /// <summary>
        /// The method that performs the actual test. This is the
        /// method that will be timed.
        /// </summary>
        /// <param name="data">Data passed to <see cref="PerformanceTester.Run(IPerformanceTest, int, object)"/></param>
        public void Run(object? data);
    }
}
