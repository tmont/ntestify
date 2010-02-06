using System;
using Moq;
using NUnit.Framework;

namespace NTestify.Tests {
	[TestFixture]
	public class ExpectedExceptionTest {

		private static Mock<ITestResult> SetupResult(string message) {
			var result = new Mock<ITestResult>();
			result
				.SetupSet(r => r.Status = TestStatus.Fail)
				.Verifiable();
			result
				.SetupSet(r => r.Message = message)
				.Verifiable();

			return result;
		}

		[NUnit.Framework.Test]
		public void Should_fail_because_exception_was_not_thrown() {
			var result = SetupResult("Expected exception of type System.Exception was never thrown.");

			var context = new ExecutionContext {
				Result = result.Object
			};

			var attribute = new ExpectedExceptionAttribute(typeof(Exception));
			attribute.Execute(context);

			result.VerifyAll();
		}

		[NUnit.Framework.Test]
		public void Should_fail_because_exception_was_wrong_type() {
			var result = SetupResult("Expected exception of type System.Exception, but exception of type System.ArgumentException was thrown.");

			var context = new ExecutionContext {
				Result = result.Object,
				ThrownException = new ArgumentException()
			};

			var attribute = new ExpectedExceptionAttribute(typeof(Exception));
			attribute.Execute(context);

			result.VerifyAll();
		}

		[NUnit.Framework.Test]
		public void Should_fail_because_exception_message_was_wrong() {
			var result = SetupResult("Expected exception message did not match actual exception message.\nExpected: foo\nActual:   bar");

			var context = new ExecutionContext {
				Result = result.Object,
				ThrownException = new Exception("bar")
			};

			var attribute = new ExpectedExceptionAttribute(typeof(Exception)) {
				ExpectedMessage = "foo"
			};
			attribute.Execute(context);

			result.VerifyAll();
		}

		[NUnit.Framework.Test]
		public void Should_pass_when_exception_matches() {
			var result = new Mock<ITestResult>();
			result
				.SetupSet(r => r.Status = TestStatus.Pass)
				.Verifiable();

			var context = new ExecutionContext {
				Result = result.Object,
				ThrownException = new Exception("foo")
			};

			var attribute = new ExpectedExceptionAttribute(typeof(Exception)) {
				ExpectedMessage = "foo"
			};
			attribute.Execute(context);

			result.VerifyAll();
		}
	}
}