using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class TestSuiteTest {

		private ExecutionContext executionContext;
		private bool testSuiteBegan, testSuiteFinished, testSuiteFailed, testSuiteIgnored, testSuitePassed;

		#region Helper methods
		private TestSuite CreateTestSuite(IEnumerable<ITest> tests) {
			var suite = new TestSuite(tests);
			suite.OnBeforeRun += context => testSuiteBegan = true;
			suite.OnAfterRun += context => testSuiteFinished = true;
			suite.OnFail += context => testSuiteFailed = true;
			suite.OnPass += context => testSuitePassed = true;
			suite.OnIgnore += context => testSuiteIgnored = true;
			return suite;
		}

		private void AssertEvents(bool passed, bool ignored, bool failed) {
			Assert.That(testSuiteBegan);
			Assert.That(testSuiteFinished);
			Assert.That(testSuitePassed, Is.EqualTo(passed));
			Assert.That(testSuiteIgnored, Is.EqualTo(ignored));
			Assert.That(testSuiteFailed, Is.EqualTo(failed));
		}

		private void AssertTestSuitePassed() {
			AssertEvents(true, false, false);
		}

		private void AssertTestSuiteWasIgnored() {
			AssertEvents(false, true, false);
		}

		private void AssertTestSuiteFailed() {
			AssertEvents(false, false, true);
		}
		#endregion

		[SetUp]
		public void SetUp() {
			executionContext = new ExecutionContext();

			testSuiteBegan = false;
			testSuiteFinished = false;
			testSuiteFailed = false;
			testSuiteIgnored = false;
			testSuitePassed = false;
		}

		[TestMethod]
		public void Should_pass_if_suite_is_empty() {
			CreateTestSuite(null).Run(executionContext);
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
		}

		[TestMethod]
		public void Should_fail_because_at_least_one_inner_test_failed() {
			var tests = new List<ITest> { new TestThatFails(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Assert.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerFailedTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Should_fail_because_at_least_one_inner_test_erred() {
			var tests = new List<ITest> { new TestThatErrs(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Assert.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.InnerErredTests.Count(), Is.EqualTo(1));
		}

		[TestMethod]
		public void Should_be_ignored_because_all_inner_tests_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Ignore));
			AssertTestSuiteWasIgnored();
			Assert.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerPassedTests.Count(), Is.EqualTo(0));
			Assert.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Should_pass_if_an_inner_test_passes_and_others_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored(), new TestThatPasses() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
			Assert.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Inner_and_outer_calculations_should_be_correct() {
			var innerInnerSuite = CreateTestSuite(new List<ITest> { new TestThatIsIgnored(), new TestThatPasses(), new TestThatErrs() });
			var innerSuite = CreateTestSuite(new List<ITest> { new TestThatFails(), new TestThatPasses(), innerInnerSuite });
			var anotherInnerSuite = CreateTestSuite(new List<ITest> { new TestThatFails(), new TestThatPasses(), new TestThatErrs() });
			var suite = CreateTestSuite(new List<ITest> { innerSuite, anotherInnerSuite, new TestThatPasses(), new TestThatFails() });

			suite.Run(executionContext);

			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));

			//check properties
			Assert.That(result.OuterResults.Count(), Is.EqualTo(4)); //two suites, two tests
			Assert.That(result.OuterCount, Is.EqualTo(4));
			Assert.That(result.OuterTests.Count(), Is.EqualTo(4));

			Assert.That(result.InnerResults.Count(), Is.EqualTo(10)); //ten tests that do not contain other tests
			Assert.That(result.InnerTests.Count(), Is.EqualTo(10));
			Assert.That(result.InnerCount, Is.EqualTo(10));

			Assert.That(result.InnerErrors.Count(), Is.EqualTo(2));
			Assert.That(result.OuterErrors.Count(), Is.EqualTo(0)); //suites can't error, and the only erring test were inside an inner suite

			Assert.That(result.InnerErredTests.Count(), Is.EqualTo(2));
			Assert.That(result.OuterErredTests.Count(), Is.EqualTo(0));

			Assert.That(result.InnerFailedTests.Count(), Is.EqualTo(3));
			Assert.That(result.OuterFailedTests.Count(), Is.EqualTo(3)); //both inner suites, and one test

			Assert.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.OuterIgnoredTests.Count(), Is.EqualTo(0)); //no suites were ignored

			Assert.That(result.InnerPassedTests.Count(), Is.EqualTo(4));
			Assert.That(result.OuterPassedTests.Count(), Is.EqualTo(1));
		}

	}

	#region Mocks
	internal class FakeTestSuite {

	}

	internal class TestThatFails : ITest {
		public void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatFails>(this) { Status = TestStatus.Fail };
		}

		public string Name { get; set; }
	}

	internal class TestThatErrs : ITest {
		public void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatErrs>(this) { Status = TestStatus.Error };
			executionContext.Result.AddError(new Exception("I AM ERROR."));
		}

		public string Name { get; set; }
	}

	internal class TestThatPasses : ITest {
		public void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatPasses>(this) { Status = TestStatus.Pass };
		}

		public string Name { get; set; }
	}

	internal class TestThatIsIgnored : ITest {
		public void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatIsIgnored>(this) { Status = TestStatus.Ignore };
		}

		public string Name { get; set; }
	}

	#endregion
}
