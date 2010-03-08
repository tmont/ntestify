using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NTestify.Configuration;

namespace NTestify.Execution {
	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods. This is the default test runner used the NTestify
	/// console and GUI applications.
	/// </summary>
	public class AssemblyTestRunner : ITestRunner {
		private readonly ICollection<IAccumulationFilter> filters;

		public AssemblyTestRunner() {
			filters = new Collection<IAccumulationFilter>();
		}

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		/// <param name="assembly">The assembly in which to search for tests</param>
		public virtual ITestResult RunAll(_Assembly assembly) {
			var suite = new AssemblyAccumulator()
				.Accumulate(assembly, Filters, Configurator)
				.Single();

			return ((ITestRunner)this).RunTest(suite, new ExecutionContext { Test = suite });
		}

		/// <summary>
		/// Configurator to use to configure each test
		/// </summary>
		public ITestConfigurator Configurator { get; set; }

		/// <summary>
		/// Runs a single test or suite and returns a result
		/// </summary>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
		}

		/// <summary>
		/// Gets all of the accumulation filters for this test runner
		/// </summary>
		public IEnumerable<IAccumulationFilter> Filters {
			get { return filters; }
		}

		/// <summary>
		/// Adds an accumulation filter to the test runner
		/// </summary>
		public ITestRunner AddFilter(IAccumulationFilter filter) {
			filters.Add(filter);
			return this;
		}

	}
}