namespace NTestify {
	/// <summary>
	/// Represents the current test execution context
	/// </summary>
	public class ExecutionContext {
		/// <summary>
		/// The object from which the tests were pulled from, i.e. the class
		/// that had the [Test] attribute.
		/// </summary>
		public object Instance { get; set; }

		/// <summary>
		/// The currently executing test
		/// </summary>
		public ITest Test { get; set; }

		/// <summary>
		/// The completed test result, or null if the test has not been run
		/// </summary>
		public ITestResult Result { get; set; }
	}
}