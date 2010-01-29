using System;

namespace NTestify {
	/// <summary>
	/// Signifies that a test should not be run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreAttribute : TestifyAttribute {
		/// <summary>
		/// The reason the test is being ignored
		/// </summary>
		public string Reason { get; set; }

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void Execute(ExecutionContext executionContext) { }
	}
}