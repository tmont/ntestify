using System;
using System.Reflection;

namespace NTestify {

	public struct FilterOrder {
		public const int BeforeSetup = 0;
		public const int AfterSetup = 2;
		public const int BeforeTearDown = -2;
		public const int AfterTearDown = 0;
	}

	/// <summary>
	/// Signifies that a test should not be run
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[Testable]
	public sealed class IgnoreAttribute : PreTestFilter {
		/// <summary>
		/// The reason the test is being ignored
		/// </summary>
		public string Reason { get; set; }

		public override int Order {
			get { return int.MinValue; }
			set { }
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class SetupAttribute : PreTestFilter {
		public override void Execute(ExecutionContext executionContext) {
			if (Method != null) {
				Method.Invoke(executionContext.Instance, new object[0]);
			}
		}

		public override int Order {
			get { return 1; }
			set { }
		}

		internal MethodInfo Method { get; set; }
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class TearDownAttribute : PostTestFilter {

		public override void Execute(ExecutionContext executionContext) {
			if (Method != null) {
				Method.Invoke(executionContext.Instance, new object[0]);
			}
		}

		public override int Order {
			get { return -1; }
			set { }
		}

		internal MethodInfo Method { get; set; }
	}
}