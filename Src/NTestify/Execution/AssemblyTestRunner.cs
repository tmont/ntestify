using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using NTestify.Configuration;

namespace NTestify.Execution {

	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods. This is the default test runner used by the NTestify
	/// console application.
	/// </summary>
	public class AssemblyTestRunner : ITestRunner {
		private readonly ICollection<IAccumulationFilter> filters;

		public AssemblyTestRunner() {
			filters = new Collection<IAccumulationFilter>();
		}

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		/// <param name="assembly">The assembly to scan for tests</param>
		public ITestResult RunAll(_Assembly assembly) {
			var test = CreateTest(assembly);
			return ((ITestRunner)this).RunTest(test, CreateContext(test));
		}

		/// <summary>
		/// Creates the assembly suite
		/// </summary>
		/// <param name="assembly">The assembly to scan for tests</param>
		protected virtual ITest CreateTest(_Assembly assembly) {
			var test = new AssemblySuite(assembly, Filters, Configurator);
			test.Configure(Configurator);
			return test;
		}

		/// <summary>
		/// Creates the execution context
		/// </summary>
		/// <param name="test">The assembly suite</param>
		protected virtual ExecutionContext CreateContext(ITest test) {
			return new ExecutionContext();
		}

		/// <summary>
		/// Configurator to use to configure each test
		/// </summary>
		public ITestConfigurator Configurator { get; set; }

		/// <summary>
		/// Runs a single test or suite and returns a result
		/// </summary>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			executionContext.Test = test;
			test.Run(executionContext);
			return executionContext.Result;
		}

		/// <summary>
		/// Gets all of the accumulation filters for this test runner
		/// </summary>
		public IEnumerable<IAccumulationFilter> Filters { get { return filters; } }

		/// <summary>
		/// Adds an accumulation filter to the test runner
		/// </summary>
		public ITestRunner AddFilter(IAccumulationFilter filter) {
			filters.Add(filter);
			return this;
		}

	}
}