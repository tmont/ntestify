using System;

namespace NTestify {
	/// <summary>
	/// Signifies that a test should not be run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[Testable]
	public class IgnoreAttribute : PreTestFilter {
		/// <summary>
		/// The reason the test is being ignored
		/// </summary>
		public string Reason { get; set; }
	}
}