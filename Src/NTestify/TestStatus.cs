namespace NTestify {
	/// <summary>
	/// Indicates the status of a test
	/// </summary>
	public enum TestStatus {
		/// <summary>
		/// The test passed
		/// </summary>
		Pass,
		/// <summary>
		/// The test failed
		/// </summary>
		Fail,
		/// <summary>
		/// The test encoutered an error that prevented it from
		/// completing normally
		/// </summary>
		Error,
		/// <summary>
		/// The test is currently running
		/// </summary>
		Running,
		/// <summary>
		/// The test was ignored
		/// </summary>
		Ignore,
		/// <summary>
		/// The test has not run
		/// </summary>
		HasNotRun
	}
}