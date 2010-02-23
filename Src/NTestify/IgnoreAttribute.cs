using System;

namespace NTestify {
	/// <summary>
	/// Signifies that a test should not be run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[PreTestFilter]
	public sealed class IgnoreAttribute : TestFilter {
		/// <summary>
		/// The reason the test is being ignored
		/// </summary>
		public string Reason { get; set; }

		public override int Order {
			get { return int.MinValue; }
			set { }
		}
	}
}