namespace NTestify {
	/// <summary>
	/// Interface for test suites, which represent a container for
	/// many tests
	/// </summary>
	public interface ITestSuite : ITest {
		/// <summary>
		/// Adds a test to the suite. Note that the test can also
		/// be another test suite
		/// </summary>
		/// <param name="test">The test</param>
		ITestSuite AddTest(ITest test);
	}
}