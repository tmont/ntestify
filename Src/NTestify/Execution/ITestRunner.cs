using System.Collections.Generic;

namespace NTestify.Execution {
	/// <summary>
	/// Interface for objects that will run tests and/or suites
	/// </summary>
	public interface ITestRunner {
		/// <summary>
		/// Runs a single test or suite and returns a result
		/// </summary>
		ITestResult RunTest(ITest test, ExecutionContext executionContext);

		/// <summary>
		/// Gets all of the accumulation filters for this test runner
		/// </summary>
		IEnumerable<IAccumulationFilter> Filters { get; }

		/// <summary>
		/// Adds an accumulation filter to the test runner
		/// </summary>
		ITestRunner AddFilter(IAccumulationFilter filter);
	}
}