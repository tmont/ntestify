using System;
using System.Linq;
using NUnit.Framework;

namespace NTestify.Tests {
	[TestFixture]
	public class Test {

		private ILogger logger;
		private object instance;
		private bool beforeTest, afterTest, testFailed, testPassed, testErred, testIgnored;

		[SetUp]
		public void SetUp() {
			logger = new Logger();
			instance = new FakeTestClass();

			beforeTest = false;
			afterTest = false;
			testFailed = false;
			testPassed = false;
			testErred = false;
			testIgnored = false;
		}

		private TestResult RunTest(string name) {
			return GetTest(name).Run(new ExecutionContext { Instance = instance });
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
			var result = RunTest("TestMethodThatPasses");
			Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
			AssertEvents(true, true, true, false, false, false);
		}

		[Test]
		public void Should_run_test_and_err() {
			var result = RunTest("TestMethodThatErrs");
			Assert.That(result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(result.Errors.Count(), Is.EqualTo(1));
			Assert.That(result.Errors.First().Message, Is.EqualTo("hi there!"));
			AssertEvents(true, true, false, false, false, true);
		}

		[Test]
		public void Should_not_run_test_due_to_invalid_method() {
			var result = RunTest("TestMethodThatIsInvalid");
			Assert.That(result.Status, Is.EqualTo(TestStatus.Error));
			Assert.That(result.Message, Is.EqualTo("Method is invalid"));
			AssertEvents(false, false, false, false, false, true);
		}

		[Test]
		public void Should_run_test_and_fail(){
			var result = RunTest("TestMethodThatFails");
			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertEvents(true, true, false, false, true, false);
		}

		[Test]
		public void Should_ignore_test() {
			var result = RunTest("TestMethodThatIsIgnored");
			Assert.That(result.Status, Is.EqualTo(TestStatus.Ignore));
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

		public void TestMethodThatIsIgnored(){
			throw new TestIgnoredException();
		}
	}
}
