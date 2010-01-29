namespace NTestify {
	public interface ITest {
		/// <summary>
		/// Runs the test
		/// </summary>
		/// <param name="executionContext">The current test execution context</param>
		TestResult Run(ExecutionContext executionContext);
	}
}