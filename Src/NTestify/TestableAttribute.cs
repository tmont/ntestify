using System;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Methods and classes with subclasses of this attribute are testable by the framework
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public abstract class TestableAttribute : TestifyAttribute {
		public override void Execute(ExecutionContext executionContext) { }
	}
}