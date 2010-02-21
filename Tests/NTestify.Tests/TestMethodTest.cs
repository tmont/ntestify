using System;
using System.Linq;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class TestMethodTest {

		private object instance;
		private bool beforeTest, afterTest, testFailed, testPassed, testErred, testIgnored;
		private ExecutionContext executionContext;

		[SetUp]
		public void SetUp() {
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

		private ReflectedTestMethod GetTest(string name) {
			var method = instance.GetType().GetMethod(name);
			var test = new ReflectedTestMethod(method, instance);

			test.OnBeforeRun += context => beforeTest = true;
			test.OnAfterRun += context => afterTest = true;
			test.OnError += context => testErred = true;
			test.OnFail += context => testFailed = true;
			test.OnPass += context => testPassed = true;
			test.OnIgnore += context => testIgnored = true;

			return test;
		}

		private void AssertEvents(bool passed, bool ignored, bool failed, bool erred) {
			Ass.That(beforeTest);
			Ass.That(afterTest);
			Ass.That(testPassed, Is.EqualTo(passed));
			Ass.That(testIgnored, Is.EqualTo(ignored));
			Ass.That(testFailed, Is.EqualTo(failed));
			Ass.That(testErred, Is.EqualTo(erred));
		}

		[TestMethod]
		public void Should_run_test_and_pass() {
			RunTest("TestMethodThatPasses");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			AssertEvents(true, false, false, false);
		}

		[TestMethod]
		public void Should_run_test_and_err() {
			RunTest("TestMethodThatErrs");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Ass.That(executionContext.Result.Errors.Count(), Is.EqualTo(1));
			Ass.That(executionContext.Result.Errors.First().Message, Is.EqualTo("hi there!"));
			AssertEvents(false, false, false, true);
		}

		[TestMethod]
		public void Invalid_method_should_err() {
			RunTest("TestMethodThatIsInvalid");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Ass.That(executionContext.Result.Message, Is.EqualTo("The test method is invalid"));
			AssertEvents(false, false, false, true);
		}

		[TestMethod]
		public void Should_run_test_and_fail() {
			RunTest("TestMethodThatFails");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Fail));
			AssertEvents(false, false, true, false);
		}

		[TestMethod]
		public void Should_ignore_test_and_bypass_all_other_filters() {
			RunTest("TestMethodThatIsIgnored");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Ignore));
			Ass.That(executionContext.Result.Message, Is.EqualTo("this test sux!"));
			AssertEvents(false, true, false, false);
		}

		[TestMethod]
		public void Should_err_when_filter_throws() {
			RunTest("TestMethodThatHasABadFilter");

			const string message = "Encountered an error while trying to run method filter of type \"NTestify.Tests.FilterThatThrowsException\"";

			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Error));
			Ass.That(executionContext.Result.Message, Is.EqualTo(message));
			Ass.That(executionContext.Result.Errors.Count(), Is.EqualTo(1));
			Ass.That(executionContext.Result.Errors.First().Message, Is.EqualTo("OH HAI!"));
			AssertEvents(false, false, false, true);
		}

		[TestMethod]
		public void Should_execute_filters_and_continue() {
			RunTest("TestMethodThatHasFilters");
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			Ass.That(FilterThatSetsProperty.Executed, "Filter never got executed, oh noes!!");
			AssertEvents(true, false, false, false);
		}

		[TestMethod]
		public void Should_set_attribute_properties() {
			var test = new ReflectedTestMethod(typeof(FakeTestClass).GetMethod("TestMethodThatHasName"), new FakeTestClass());
			Ass.That(test.Name, Is.EqualTo("lolz"));
			Ass.That(test.Category, Is.EqualTo("Da category"));
			Ass.That(test.Description, Is.EqualTo("A test that has a name"));
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

		[Test(Name = "lolz", Category = "Da category", Description = "A test that has a name")]
		public void TestMethodThatHasName() { }
	}

	[PreTestFilter]
	internal class FilterThatThrowsException : TestFilter {
		public override void Execute(ExecutionContext executionContext) {
			throw new Exception("OH HAI!");
		}
	}

	[PreTestFilter]
	internal class FilterThatSetsProperty : TestFilter {
		public override void Execute(ExecutionContext executionContext) {
			Executed = true;
		}

		public static bool Executed { get; private set; }
	}
}
