using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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

		[Test]
		public void Should_pass_if_suite_is_empty() {
			CreateTestSuite(null).Run(executionContext);
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
		}

		[Test]
		public void Should_fail_because_at_least_one_inner_test_failed() {
			var tests = new List<ITest> { new TestThatFails(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Assert.That(result.IgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.PassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.FailedTests.Count(), Is.EqualTo(1));
			Assert.That(result.ErredTests.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Should_fail_because_at_least_one_inner_test_erred() {
			var tests = new List<ITest> { new TestThatErrs(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Assert.That(result.IgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.PassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.FailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.ErredTests.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Should_be_ignored_because_all_inner_tests_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Ignore));
			AssertTestSuiteWasIgnored();
			Assert.That(result.IgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.PassedTests.Count(), Is.EqualTo(0));
			Assert.That(result.FailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.ErredTests.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Should_pass_if_an_inner_test_passes_and_others_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored(), new TestThatPasses() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
			Assert.That(result.IgnoredTests.Count(), Is.EqualTo(1));
			Assert.That(result.PassedTests.Count(), Is.EqualTo(1));
			Assert.That(result.FailedTests.Count(), Is.EqualTo(0));
			Assert.That(result.ErredTests.Count(), Is.EqualTo(0));
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
