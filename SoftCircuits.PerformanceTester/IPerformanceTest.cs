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
        /// Method that is called before the test to perform any
        /// initialization.  This time taken by this method is not
        /// included in the timing of this test.
        /// </summary>
        void Initialize();

        /// <summary>
        /// The method that performs the actual test. This is the
        /// method that will be timed.
        /// </summary>
        void Run();
    }
}
