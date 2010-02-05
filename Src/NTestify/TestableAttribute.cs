using System;

namespace NTestify {

	/// <summary>
	/// When applied to another attribute, indicates that that attribute
	/// will mark whatever it's annotating as a runnable test. For example,
	/// this attribute is applied to IgnoreAttribute and TestAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TestableAttribute : Attribute { }

	/// <summary>
	/// When a method or class is annotated with a subclass, this filter will be executed after
	/// a test runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class PostTestFilter : TestifyAttribute { }

	/// <summary>
	/// When a method or class is annotated with a subclass, this filter will be executed before
	/// a test runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class PreTestFilter : TestifyAttribute { }

	/// <summary>
	/// Indicates that a test method is expected to throw an exception
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExpectedExceptionAttribute : PostTestFilter {

		/// <summary>
		/// The expected exception message
		/// </summary>
		public string ExpectedMessage { get; set; }
		/// <summary>
		/// The type of exception that is expected to be thrown
		/// </summary>
		public Type ExpectedException { get; protected set; }

		/// <param name="expectedException">The type of exception that is expected to be thrown</param>
		public ExpectedExceptionAttribute(Type expectedException) {
			ExpectedException = expectedException;
		}

		///<inheritdoc/>
		public override void Execute(ExecutionContext executionContext) {
			if (executionContext.ThrownException == null) {
				Fail(executionContext, string.Format("Expected exception of type {0} was never thrown.", ExpectedException.GetFriendlyName()));
				return;
			}

			var exception = executionContext.ThrownException;
			if (exception.GetType() != ExpectedException) {
				Fail(executionContext, string.Format(
					"Expected exception of type {0}, but exception of type {1} was thrown.", 
					ExpectedException.GetFriendlyName(), 
					exception.GetType().GetFriendlyName()
				));

				return;
			}

			if (!string.IsNullOrEmpty(ExpectedMessage)) {
				if (exception.Message != ExpectedMessage) {
					var message = string.Format(
						"Expected exception message did not match actual exception message.\nExpected: {0}\nActual:   {1}",
						ExpectedMessage,
						exception.Message
					);

					Fail(executionContext, message);
					return;
				}
			}
		}

		/// <summary>
		/// Sets the result status to fail
		/// </summary>
		protected void Fail(ExecutionContext executionContext, string reasonForFailure) {
			executionContext.Result.Status = TestStatus.Fail;
			executionContext.Result.Message = reasonForFailure;
		}
	}
}