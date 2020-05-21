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
        /// <summary>
        /// Runs all the tests (classes that implement the <see cref="IPerformanceTest"/>
        /// interface) in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to find and run the
        /// tests.</param>
        /// <returns>Returns all the test results.</returns>
        public IEnumerable<TestResult> Run(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            Stopwatch stopwatch = new Stopwatch();
            List<TestResult> results = new List<TestResult>();
            foreach (Type type in GetAssemblyTests(assembly))
            {
                // Create an instance of this class
                IPerformanceTest test = (IPerformanceTest)Activator.CreateInstance(type);

                // Run this test
                test.Initialize();
                stopwatch.Restart();
                test.Run();
                stopwatch.Stop();

                // Add the result
                results.Add(new TestResult
                {
                    Description = test.Description,
                    Milliseconds = stopwatch.ElapsedMilliseconds,
                });
            }

            // Calculate result percents (percent of slowest result)
            // Avoid divide by zero
            long maxMilliseconds = results.Select(r => r.Milliseconds).DefaultIfEmpty(0).Max();
            if (maxMilliseconds == 0)
                results.ForEach(r => r.Percent = 0);
            else
                results.ForEach(r => r.Percent = Math.Min((int)((double)r.Milliseconds / maxMilliseconds * 100), 100));

            return results;
        }

        /// <summary>
        /// Gets all the types derived from <see cref="IPerformanceTest"/>
        /// from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to get the types.</param>
        /// <returns>The list of found types.</returns>
        private IEnumerable<Type> GetAssemblyTests(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IPerformanceTest)));
        }
    }
}
