using System;

namespace NTestify {
	/// <summary>
	/// When a method is annotated with [TearDown], it will be invoked after
	/// each test method is run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[PostTestFilter]
	public sealed class TearDownAttribute : InvokableFilter {
		public override int Order {
			get { return -1; }
			set { throw new InvalidOperationException("Order of teardown attributes cannot change"); }
		}
	}
}