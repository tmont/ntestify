using System;

namespace NTestify {
	/// <summary>
	/// Marks a class or method as a runnable test
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[Testable]
	public class TestAttribute : TestifyAttribute, ITestInfo {
		/// <summary>
		/// Description of the test
		/// </summary>
		public virtual string Description { get; set; }

		/// <summary>
		/// Name of the category for this test
		/// </summary>
		public virtual string Category { get; set; }

		/// <summary>
		/// Name of the test
		/// </summary>
		public virtual string Name { get; set; }
	}
}