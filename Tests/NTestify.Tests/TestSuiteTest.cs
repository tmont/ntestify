using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

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
			Ass.That(testSuiteBegan);
			Ass.That(testSuiteFinished);
			Ass.That(testSuitePassed, Is.EqualTo(passed));
			Ass.That(testSuiteIgnored, Is.EqualTo(ignored));
			Ass.That(testSuiteFailed, Is.EqualTo(failed));
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
			Ass.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
		}

		[TestMethod]
		public void Should_fail_because_at_least_one_inner_test_failed() {
			var tests = new List<ITest> { new TestThatFails(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Ass.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Ass.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerFailedTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Should_fail_because_at_least_one_inner_test_erred() {
			var tests = new List<ITest> { new TestThatErrs(), new TestThatPasses(), new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Ass.That(result.Status, Is.EqualTo(TestStatus.Fail));
			AssertTestSuiteFailed();
			Ass.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Ass.That(result.InnerErredTests.Count(), Is.EqualTo(1));
		}

		[TestMethod]
		public void Should_be_ignored_because_all_inner_tests_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Ass.That(result.Status, Is.EqualTo(TestStatus.Ignore));
			AssertTestSuiteWasIgnored();
			Ass.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerPassedTests.Count(), Is.EqualTo(0));
			Ass.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Ass.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Should_pass_if_an_inner_test_passes_and_others_were_ignored() {
			var tests = new List<ITest> { new TestThatIsIgnored(), new TestThatPasses() };
			CreateTestSuite(tests).Run(executionContext);
			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Ass.That(result.Status, Is.EqualTo(TestStatus.Pass));
			AssertTestSuitePassed();
			Ass.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerPassedTests.Count(), Is.EqualTo(1));
			Ass.That(result.InnerFailedTests.Count(), Is.EqualTo(0));
			Ass.That(result.InnerErredTests.Count(), Is.EqualTo(0));
		}

		[TestMethod]
		public void Inner_and_outer_calculations_should_be_correct() {
			var innerInnerSuite = CreateTestSuite(new List<ITest> { new TestThatIsIgnored(), new TestThatPasses(), new TestThatErrs() });
			var innerSuite = CreateTestSuite(new List<ITest> { new TestThatFails(), new TestThatPasses(), innerInnerSuite });
			var anotherInnerSuite = CreateTestSuite(new List<ITest> { new TestThatFails(), new TestThatPasses(), new TestThatErrs() });
			var suite = CreateTestSuite(new List<ITest> { innerSuite, anotherInnerSuite, new TestThatPasses(), new TestThatFails() });

			suite.Run(executionContext);

			var result = executionContext.Result.CastTo<TestSuiteResult>();

			Ass.That(result.Status, Is.EqualTo(TestStatus.Fail));

			//check properties
			Ass.That(result.OuterResults.Count(), Is.EqualTo(4)); //two suites, two tests
			Ass.That(result.OuterCount, Is.EqualTo(4));
			Ass.That(result.OuterTests.Count(), Is.EqualTo(4));

			Ass.That(result.InnerResults.Count(), Is.EqualTo(10)); //ten tests that do not contain other tests
			Ass.That(result.InnerTests.Count(), Is.EqualTo(10));
			Ass.That(result.InnerCount, Is.EqualTo(10));

			Ass.That(result.InnerErrors.Count(), Is.EqualTo(2));
			Ass.That(result.OuterErrors.Count(), Is.EqualTo(0)); //suites can't error, and the only erring test were inside an inner suite

			Ass.That(result.InnerErredTests.Count(), Is.EqualTo(2));
			Ass.That(result.OuterErredTests.Count(), Is.EqualTo(0));

			Ass.That(result.InnerFailedTests.Count(), Is.EqualTo(3));
			Ass.That(result.OuterFailedTests.Count(), Is.EqualTo(3)); //both inner suites, and one test

			Ass.That(result.InnerIgnoredTests.Count(), Is.EqualTo(1));
			Ass.That(result.OuterIgnoredTests.Count(), Is.EqualTo(0)); //no suites were ignored

			Ass.That(result.InnerPassedTests.Count(), Is.EqualTo(4));
			Ass.That(result.OuterPassedTests.Count(), Is.EqualTo(1));
		}

	}

	#region Mocks
	internal class FakeTestSuite {

	}

	internal abstract class FakeTest : ITest {
		public abstract void Run(ExecutionContext executionContext);

		public string Name { get; set; }
		public ITestConfigurator Configurator { get; set; }
		public ITest Configure(ITestConfigurator configurator){
			return this;
		}

		public event Action<ExecutionContext> OnBeforeRun;
		public event Action<ExecutionContext> OnAfterRun;
		public event Action<ExecutionContext> OnIgnore;
		public event Action<ExecutionContext> OnPass;
		public event Action<ExecutionContext> OnFail;
		public event Action<ExecutionContext> OnError;
	}

	internal class TestThatFails : FakeTest {
		public override void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatFails>(this) { Status = TestStatus.Fail };
		}
	}

	internal class TestThatErrs : FakeTest {
		public override void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatErrs>(this) { Status = TestStatus.Error };
			executionContext.Result.AddError(new Exception("I AM ERROR."));
		}
	}

	internal class TestThatPasses : FakeTest {
		public override void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatPasses>(this) { Status = TestStatus.Pass };
		}
	}

	internal class TestThatIsIgnored : FakeTest {
		public override void Run(ExecutionContext executionContext) {
			executionContext.Result = new TestResult<TestThatIsIgnored>(this) { Status = TestStatus.Ignore };
		}
	}

	#endregion
}
