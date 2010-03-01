using System;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class ExpectedExceptionTest {

		private Mock<Test> test;
		private readonly Exception exception = new Exception("I AM ERROR.");

		[SetUp]
		public void SetUp() {
			test = new Mock<Test> { CallBase = true };
			test.Protected().Setup<ITestResult>("CreateTestResult").Returns(new TestResult<Test>(test.Object));
		}

		[TestMethod]
		public void Should_fail_because_exception_was_not_thrown() {
			var testObject = test.Object;
			testObject.ExpectedException = typeof(Exception);

			var context = new ExecutionContext();
			testObject.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Fail));
			Ass.That(context.Result.Message, Is.EqualTo("Expected exception of type System.Exception was never thrown."));
		}

		[TestMethod]
		public void Should_fail_because_exception_was_wrong_type() {
			var context = new ExecutionContext();
			test.Protected().Setup("RunTest", context).Throws(new ArgumentException());
			var testObject = test.Object;
			testObject.ExpectedException = typeof(Exception);

			
			testObject.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Fail));
			Ass.That(context.Result.Message, Is.EqualTo("Expected exception of type System.Exception, but exception of type System.ArgumentException was thrown."));
		}

		[TestMethod]
		public void Should_fail_because_exception_message_was_wrong() {
			var context = new ExecutionContext();
			test.Protected().Setup("RunTest", context).Throws(new Exception("bar"));

			var testObject = test.Object;
			testObject.ExpectedException = typeof(Exception);
			testObject.ExpectedExceptionMessage = "foo";
			testObject.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Fail));
			Ass.That(context.Result.Message, Is.EqualTo("Expected exception message did not match actual exception message.\nExpected: foo\nActual:   bar"));
		}

		[TestMethod]
		public void Should_pass_when_exception_matches() {
			var context = new ExecutionContext();
			test.Protected().Setup("RunTest", context).Throws(new Exception("foo"));

			var testObject = test.Object;
			testObject.ExpectedException = typeof(Exception);
			testObject.ExpectedExceptionMessage = "foo";
			testObject.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Pass));
		}

		[TestMethod]
		public void Should_use_cause_error_if_exception_is_TestErredException() {
			var context = new ExecutionContext();
			test.Protected().Setup("RunTest", context).Throws(new Test.TestErredException(new Exception("I can haz lulz"), "An error occurred"));

			var testObject = test.Object;
			testObject.ExpectedException = typeof(Exception);
			testObject.ExpectedExceptionMessage = "I can haz lulz";
			testObject.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Pass));
		}
	}
}