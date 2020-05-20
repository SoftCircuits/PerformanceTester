// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.PerformanceTester
{
    public interface IPerformanceTest
    {
        string Description { get; }
        void Initialize();
        void Run();
    }
}
