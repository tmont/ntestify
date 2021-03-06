using System;

namespace NTestify {
	/// <summary>
	/// When a method is annotated with [Setup], it will be invoked before
	/// each test method is run
	/// </summary>
	[PreTestFilter]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SetupAttribute : InvokableFilter {
		public override int Order {
			get { return 1; }
			set { throw new InvalidOperationException("Order of setup attributes cannot change"); }
		}
	}

	/// <summary>
	/// When a method is annotated with [SuiteSetup], it will be invoked before
	/// the suite is run
	/// </summary>
	[PreSuiteFilter]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SuiteSetupAttribute : InvokableFilter { }

	/// <summary>
	/// When a method is annotated with [SuiteTearDown], it will be invoked
	/// after the suite is run
	/// </summary>
	[PostSuiteFilter]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SuiteTearDownAttribute : InvokableFilter { }
}