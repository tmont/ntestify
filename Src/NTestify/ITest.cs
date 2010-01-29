namespace NTestify {
	public interface ITest {
		/// <summary>
		/// Runs the test, and sets the ExecutionContext's Result property
		/// with the result of the test
		/// </summary>
		/// <param name="executionContext">The current test execution context</param>
		void Run(ExecutionContext executionContext);
	}
}