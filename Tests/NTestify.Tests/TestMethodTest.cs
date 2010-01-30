using System;
using System.Linq;
using NTestify.Logging;
using NUnit.Framework;

namespace NTestify.Tests {
	[TestFixture]
	public class TestMethodTest {

		private ILogger logger;
		private object instance;
		private bool beforeTest, afterTest, testFailed, testPassed, testErred, testIgnored;
		private ExecutionContext executionContext;

		[SetUp]
		public void SetUp() {
			logger = new Logger();
			instance = new FakeTestClass();
			executionContext = new ExecutionContext { Instance = instance };

			beforeTest = false;
			afterTest = false;
			testFailed = false;
			testPassed = false;
			testErred = false;
			testIgnored = false;
		}

		private void RunTest(string name) {
			GetTest(name).Run(executionContext);
		}

		private TestMethod GetTest(string name) {
			var method = instance.GetType().GetMethod(name);
			var test = new TestMethod(method);
			test.SetLogger(logger);

			test.OnBeforeRun += context => beforeTest = true;
			test.OnAfterRun += context => afterTest = true;
			test.OnError += context => testErred = true;
			test.OnFail += context => testFailed = true;
			test.OnPass += context => testPassed = true;
			test.OnIgnore += context => testIgnored = true;

			return test;
		}

		private void AssertEvents(bool before, bool after, bool passed, bool ignored, bool failed, bool erred) {
			Assert.That(beforeTest, Is.EqualTo(before));
			Assert.That(afterTest, Is.EqualTo(after));
			Assert.That(testPassed, Is.EqualTo(passed));
			Assert.That(testIgnored, Is.EqualTo(ignored));
			Assert.That(testFailed, Is.EqualTo(failed));
			Assert.That(testErred, Is.EqualTo(erred));
		}

		[Test]
		public void Should_run_test_and_pass() {
			RunTest("TestMethodThatPasses");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			AssertEvents(true, true, true, false, false, false);
		}

		[Test]
		public void Should_run_test_and_err() {
			RunTest("TestMethodThatErrs");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(executionContext.Result.Errors.Count(), Is.EqualTo(1));
			Assert.That(executionContext.Result.Errors.First().Message, Is.EqualTo("hi there!"));
			AssertEvents(true, true, false, false, false, true);
		}

		[Test]
		public void Invalid_method_should_err() {
			RunTest("TestMethodThatIsInvalid");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(executionContext.Result.Message, Is.EqualTo("The test method is invalid"));
			AssertEvents(true, true, false, false, false, true);
		}

		[Test]
		public void Should_run_test_and_fail() {
			RunTest("TestMethodThatFails");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Fail));
			AssertEvents(true, true, false, false, true, false);
		}

		[Test]
		public void Should_ignore_test_and_bypass_all_other_filters() {
			RunTest("TestMethodThatIsIgnored");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Ignore));
			Assert.That(executionContext.Result.Message, Is.EqualTo("this test sux!"));
			AssertEvents(true, true, false, true, false, false);
		}

		[Test]
		public void Should_err_when_filter_throws() {
			RunTest("TestMethodThatHasABadFilter");

			const string expectedMessage = "Encountered an error while trying to run method filter \"NTestify.Tests.FilterThatThrowsException\"";

			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(executionContext.Result.Message, Is.EqualTo(expectedMessage));
			Assert.That(executionContext.Result.Errors.Count(), Is.EqualTo(1));
			Assert.That(executionContext.Result.Errors.First().Message, Is.EqualTo("OH HAI!"));
			AssertEvents(true, true, false, false, false, true);
		}

		[Test]
		public void Should_execute_filters_and_continue() {
			RunTest("TestMethodThatHasFilters");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			Assert.That(FilterThatSetsProperty.Executed, "Filter never got executed, oh noes!!");
			AssertEvents(true, true, true, false, false, false);
		}

	}

	internal class FakeTestClass {
		public void TestMethodThatPasses() { }

		public void TestMethodThatErrs() {
			throw new Exception("hi there!");
		}

		public void TestMethodThatFails() {
			throw new TestAssertionException();
		}

		public void TestMethodThatIsInvalid(int foo) { }

		[FilterThatThrowsException, Ignore(Reason = "this test sux!")]
		public void TestMethodThatIsIgnored() {
			throw new Exception("This should never get thrown");
		}
		[FilterThatThrowsException]
		public void TestMethodThatHasABadFilter() {
			throw new Exception("This should never get thrown");
		}

		[FilterThatSetsProperty]
		public void TestMethodThatHasFilters() { }
	}

	internal class FilterThatThrowsException : TestifyAttribute {
		public override void Execute(ExecutionContext executionContext) {
			throw new Exception("OH HAI!");
		}
	}

	internal class FilterThatSetsProperty : TestifyAttribute {
		public override void Execute(ExecutionContext executionContext) {
			Executed = true;
		}

		public static bool Executed { get; private set; }
	}
}
