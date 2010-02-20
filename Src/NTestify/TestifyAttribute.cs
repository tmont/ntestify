using System;

namespace NTestify {
	/// <summary>
	/// Base class for all testify attributes
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class TestifyAttribute : Attribute {
		/// <summary>
		/// Executes some action with the current test's execution context
		/// </summary>
		public virtual void Execute(ExecutionContext executionContext) { }
	}
}