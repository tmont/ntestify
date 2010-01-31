namespace NTestify {
	/// <summary>
	/// Interface for objects that will run tests and/or suites
	/// </summary>
	public interface ITestRunner {
		/// <summary>
		/// Runs a single test or suite and returns a result
		/// </summary>
		ITestResult RunTest(ITest test, ExecutionContext executionContext);
	}
}