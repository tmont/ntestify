using System;

namespace NTestify {
	/// <summary>
	/// Base class for all testify attributes
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class TestifyAttribute : Attribute {
		/// <summary>
		/// This method gets called before running a test
		/// </summary>
		public virtual void Execute(ExecutionContext executionContext) { }
	}
}