using System;

namespace NTestify {
	/// <summary>
	/// Indicates that a test method is expected to throw an exception
	/// TODO: this logic shouldn't be here, it should be in Test
	/// </summary>
	[PostTestFilter]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExpectedExceptionAttribute : TestFilter {

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

			var temp = executionContext.ThrownException.GetBaseException();
			var exception = (temp is Test.TestErredException && ((Test.TestErredException)temp).CauseError != null) ? ((Test.TestErredException)temp).CauseError : temp;

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

			executionContext.Result.Status = TestStatus.Pass;
		}

		/// <summary>
		/// Sets the result status to fail
		/// </summary>
		private static void Fail(ExecutionContext executionContext, string reasonForFailure) {
			executionContext.Result.Status = TestStatus.Fail;
			executionContext.Result.Message = reasonForFailure;
		}
	}
}