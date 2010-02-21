namespace NTestify {
	/// <summary>
	/// Specifies the order of operations for test filters
	/// </summary>
	public struct FilterOrder {
		/// <summary>
		/// The pre filter should be run before setup (default)
		/// </summary>
		public const int BeforeSetup = 0;
		/// <summary>
		/// The pre filter should be run after setup
		/// </summary>
		public const int AfterSetup = 2;
		/// <summary>
		/// The post filter should be run before tear down
		/// </summary>
		public const int BeforeTearDown = -2;
		/// <summary>
		/// The post filter should be run after tear down (default)
		/// </summary>
		public const int AfterTearDown = 0;
	}
}