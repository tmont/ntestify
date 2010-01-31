using System;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Marks a class or method as a test
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class TestAttribute : TestableAttribute {
		/// <summary>
		/// Logger for this object
		/// </summary>
		protected ILogger Logger { get; private set; }

		/// <summary>
		/// Description of the test
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Name of the category for this test
		/// </summary>
		public string Category { get; set; }

		public TestAttribute()
			: this(new NullLogger()) {

		}

		public TestAttribute(ILogger logger){
			Logger = logger;
		}

		

		///<inheritdoc/>
		public override void Execute(ExecutionContext executionContext) {
			Logger.Debug("About to run test [" + executionContext.Test.Name + "]");
		}
	}
}