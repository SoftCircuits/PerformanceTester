// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SoftCircuits.PerformanceTester
{
    public class PerformanceTester
    {
        private readonly List<TestResult> TestResults;

        /// <summary>
        /// Constructs a new <see cref="PerformanceTester"/> instance.
        /// </summary>
        public PerformanceTester()
        {
            TestResults = new List<TestResult>();
        }

        /// <summary>
        /// Runs the specified test.
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="test"/>
        /// is <c>null</c>.</exception>
        public IEnumerable<TestResult> Run(IPerformanceTest test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            PrepareTests();
            InternalRun(test);
            ConcludeTests();
            return TestResults;
        }

        /// <summary>
        /// Runs the specified tests.
        /// </summary>
        /// <param name="tests">The tests to run.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tests"/>
        /// is <c>null</c>.</exception>
        public IEnumerable<TestResult> Run(params IPerformanceTest[] tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            PrepareTests();
            foreach (IPerformanceTest test in tests)
                InternalRun(test);
            ConcludeTests();
            return TestResults;
        }

        /// <summary>
        /// Runs a test of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of test to run.</typeparam>
        /// <returns>The test results.</returns>
        public IEnumerable<TestResult> Run<T>() where T : IPerformanceTest, new()
        {
            IPerformanceTest test = Activator.CreateInstance<T>();
            PrepareTests();
            InternalRun(test);
            ConcludeTests();
            return TestResults;
        }

        /// <summary>
        /// Runs a test of each of the specified types.
        /// </summary>
        /// <param name="types">The list of test types to run. Each type must implement
        /// <see cref="IPerformanceTest"/> or an exception is thrown.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="InvalidOperationException">One or more type does not implement
        /// <see cref="IPerformanceTest"/>.</exception>
        public IEnumerable<TestResult> Run(params Type[] types)
        {
            PrepareTests();
            foreach (Type type in types)
            {
                if (!type.GetInterfaces().Contains(typeof(IPerformanceTest)))
                    throw new InvalidOperationException($"The test '{type.FullName}' does not implement '{typeof(IPerformanceTest).FullName}'.");
                IPerformanceTest test = (IPerformanceTest)Activator.CreateInstance(type);
                InternalRun(test);
            }
            ConcludeTests();
            return TestResults;
        }

        /// <summary>
        /// Runs a test of each of the types found in <paramref name="assembly"/> that
        /// implement <see cref="IPerformanceTest"/>.
        /// </summary>
        /// <param name="assembly">The assembly from which to find and run
        /// tests.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is
        /// <c>null</c>.</exception>
        public IEnumerable<TestResult> Run(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return Run(assembly
                .GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IPerformanceTest)))
                .ToArray());
        }

        /// <summary>
        /// Internal method that runs a single test.
        /// </summary>
        /// <param name="test">The test to run.</param>
        private void InternalRun(IPerformanceTest test)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Run this test
            stopwatch.Start();
            test.Run();
            stopwatch.Stop();

            // Add the result
            TestResults.Add(new TestResult
            {
                Description = test.Description,
                Milliseconds = stopwatch.ElapsedMilliseconds,
            });
        }

        /// <summary>
        /// Prepares the results to run one or more tests.
        /// </summary>
        private void PrepareTests()
        {
            TestResults.Clear();
        }

        /// <summary>
        /// Finalizes the results after running one or more tests.
        /// </summary>
        private void ConcludeTests()
        {
            // Calculate result percentages (as a percent of slowest result)
            long maxMilliseconds = TestResults.Select(r => r.Milliseconds).DefaultIfEmpty(0).Max();
            // Avoid divide by zero
            if (maxMilliseconds == 0)
                TestResults.ForEach(r => r.Percent = 0);
            else
                TestResults.ForEach(r => r.Percent = Math.Min((int)((double)r.Milliseconds / maxMilliseconds * 100), 100));
        }
    }
}
