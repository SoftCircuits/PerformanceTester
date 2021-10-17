// Copyright (c) 2020-2021 Jonathan Wood (www.softcircuits.com)
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
        /// If true and the test runs multiple iterations,
        /// the test results will show the average time rather than the total time.
        /// </summary>
        public bool AverageResults { get; set; }

        /// <summary>
        /// Constructs a new <see cref="PerformanceTester"/> instance.
        /// </summary>
        /// <param name="averageResults">If true and the test runs multiple iterations,
        /// the test results will show the average time rather than the total time.</param>
        public PerformanceTester(bool averageResults = false)
        {
            TestResults = new List<TestResult>();
            AverageResults = averageResults;
        }

        #region IPerformanceTest arguments

        /// <summary>
        /// Runs the specified test.
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <param name="iterations">Number of times to run the test.</param>
        /// <param name="data">Optional data to pass to the test.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="test"/>
        /// is <c>null</c>.</exception>
        public IEnumerable<TestResult> Run(IPerformanceTest test, int iterations = 1, object? data = null)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            PrepareTests();
            InternalRun(test, iterations, data);
            ConcludeTests(iterations);
            return TestResults;
        }

        /// <summary>
        /// Runs the specified tests.
        /// </summary>
        /// <param name="tests">The tests to run.</param>
        /// <param name="iterations">Number of times to run each test.</param>
        /// <param name="data">Optional data to pass to each test.</param>
        /// <returns>The test results.</returns>
        public IEnumerable<TestResult> Run(IEnumerable<IPerformanceTest> tests, int iterations = 1, object? data = null)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            PrepareTests();
            foreach (IPerformanceTest test in tests)
                InternalRun(test, iterations, data);
            ConcludeTests(iterations);
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
            return Run(tests, 1, null);
        }

        #endregion

        #region Type arguments

        /// <summary>
        /// Runs a test of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of test to run.</typeparam>
        /// <param name="iterations">Number of times to run the test.</param>
        /// <param name="data">Optional data to pass to the test.</param>
        /// <returns>The test results.</returns>
        public IEnumerable<TestResult> Run<T>(int iterations = 1, object? data = null) where T : IPerformanceTest, new()
        {
            return Run(Activator.CreateInstance<T>(), iterations, data);
        }

        /// <summary>
        /// Runs the specified tests.
        /// </summary>
        /// <param name="types">The list of test types to run. Each type must implement
        /// <see cref="IPerformanceTest"/> or an exception is thrown.</param>
        /// <returns>The test results.</param>
        /// <param name="iterations">Number of times to run each test.</param>
        /// <param name="data">Optional data to pass to each test.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="InvalidOperationException">One or more type does not implement
        /// <see cref="IPerformanceTest"/>.</exception>
        public IEnumerable<TestResult> Run(IEnumerable<Type> types, int iterations = 1, object? data = null)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            PrepareTests();
            foreach (Type type in types)
            {
                if (!type.GetInterfaces().Contains(typeof(IPerformanceTest)))
                    throw new InvalidOperationException($"The test '{type.FullName}' does not implement '{typeof(IPerformanceTest).FullName}'.");
                if (Activator.CreateInstance(type) is IPerformanceTest test)
                    InternalRun(test, iterations, data);
                else
                    throw new InvalidOperationException($"Unable to create instance of {type.FullName}.");
            }
            ConcludeTests(iterations);
            return TestResults;
        }

        /// <summary>
        /// Runs a test of each of the specified types.
        /// </summary>
        /// <param name="types">The list of test types to run. Each type must implement
        /// <see cref="IPerformanceTest"/> or an exception is thrown.</param>
        /// <param name="iterations">Number of times to run each test.</param>
        /// <param name="data">Optional data to pass to each test.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="InvalidOperationException">One or more type does not implement
        /// <see cref="IPerformanceTest"/>.</exception>
        public IEnumerable<TestResult> Run(params Type[] types)
        {
            return Run(types, 1, null);
        }

        #endregion

        #region Assembly arguments

        /// <summary>
        /// Runs a test for each type found in <paramref name="assembly"/> that
        /// implement <see cref="IPerformanceTest"/>.
        /// </summary>
        /// <param name="assembly">The assembly from which to find and run
        /// tests.</param>
        /// <param name="iterations">Number of times to run each test.</param>
        /// <param name="data">Optional data to pass to each test.</param>
        /// <returns>The test results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is
        /// <c>null</c>.</exception>
        public IEnumerable<TestResult> Run(Assembly assembly, int iterations = 1, object? data = null)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return Run(assembly
                .GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IPerformanceTest))),
                iterations,
                data);
        }

        #endregion

        /// <summary>
        /// Internal method that runs a single test.
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <param name="iterations">Number of times to run the test.</param>
        /// <param name="data">Data to pass to the test.</param>
        private void InternalRun(IPerformanceTest test, int iterations, object? data)
        {
            Stopwatch stopwatch = new();

            // Allow test class to initialize iteself
            test.Initialize(data);

            // Keep iterations positive
            iterations = Math.Max(iterations, 1);

            // Run this test
            for (int i = 0; i < iterations; i++)
            {
                stopwatch.Start();
                test.Run(data);
                stopwatch.Stop();
            }

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
        private void PrepareTests() => TestResults.Clear();

        /// <summary>
        /// Finalizes the results after running one or more tests.
        /// </summary>
        private void ConcludeTests(int iterations/* = 1*/) => TestResult.SetResultPercentages(TestResults, iterations, AverageResults);
    }
}
