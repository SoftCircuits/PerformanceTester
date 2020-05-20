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
        /// Runs all the tests in the specified assembly. Tests are implemented
        /// as classes that implement the <see cref="IPerformanceTest"/>
        /// interface.
        /// </summary>
        /// <param name="assembly">The assembly from which to find and run the
        /// tests.</param>
        public IEnumerable<TestResult> Run(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            Stopwatch stopwatch = new Stopwatch();
            List<TestResult> results = new List<TestResult>();

            foreach (Type type in GetAssemblyTests(assembly))
            {
                IPerformanceTest test = (IPerformanceTest)Activator.CreateInstance(type);

                test.Initialize();
                stopwatch.Restart();
                test.Run();
                stopwatch.Stop();

                results.Add(new TestResult
                {
                    Description = test.Description,
                    Milliseconds = stopwatch.ElapsedMilliseconds,
                });
            }

            // Calculate percents
            long max = results.Select(r => r.Milliseconds).DefaultIfEmpty(0).Max();
            foreach (TestResult result in results)
                result.Percent = Math.Min((int)((double)result.Milliseconds / max * 100), 100);

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
