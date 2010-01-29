using System;
using System.Linq;
using NUnit.Framework;

namespace NTestify.Tests {
	[TestFixture]
	public class TestMethodTest {

		private ILogger logger;
		private object instance;
		private bool beforeTest, afterTest, testFailed, testPassed, testErred, testIgnored;
		private ExecutionContext executionContext;

		[SetUp]
		public void SetUp(){
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

			test.BeforeTestRunEvent += context => beforeTest = true;
			test.AfterTestRunEvent += (context, result) => afterTest = true;
			test.TestErredEvent += (context, result) => testErred = true;
			test.TestFailedEvent += (context, result) => testFailed = true;
			test.TestPassedEvent += (context, result) => testPassed = true;
			test.TestIgnoredEvent += (context, result) => testIgnored = true;

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
		public void Should_not_run_test_due_to_invalid_method() {
			RunTest("TestMethodThatIsInvalid");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(executionContext.Result.Message, Is.EqualTo("Method is invalid"));
			AssertEvents(false, false, false, false, false, true);
		}

		[Test]
		public void Should_run_test_and_fail(){
			RunTest("TestMethodThatFails");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Fail));
			AssertEvents(true, true, false, false, true, false);
		}

		[Test]
		public void Should_ignore_test() {
			RunTest("TestMethodThatIsIgnored");
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Ignore));
			Assert.That(executionContext.Result.Message, Is.EqualTo("this test sux!"));
			AssertEvents(true, true, false, true, false, false);
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

		[Ignore(Reason = "this test sux!")]
		public void TestMethodThatIsIgnored(){
			
		}
	}
}
