using System;

namespace NTestify {
	/// <summary>
	/// When a method is annotated with [Setup], it will be invoked before
	/// each test method is run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[PreTestFilter]
	public sealed class SetupAttribute : InvokableFilter {
		public override int Order {
			get { return 1; }
			set { throw new InvalidOperationException("Order of setup attributes cannot change"); }
		}
	}
}