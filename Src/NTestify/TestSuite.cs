using System;
using System.Collections.Generic;
using System.Linq;

namespace NTestify {
	/// <summary>
	/// Represents a collection (i.e. "suite") of tests that will be run
	/// sequentially and then aggregated into one test result upon completion
	/// </summary>
	public class TestSuite : Test, ITestSuite {
		private readonly List<ITest> tests;

		/// <summary>
		/// All tests that have been added to this suite
		/// </summary>
		public IEnumerable<ITest> Tests { get { return tests; } }

		public TestSuite() {
			tests = new List<ITest>();
		}

		public TestSuite(ITest test)
			: this() {
			if (test != null) {
				tests.Add(test);
			}
		}

		public TestSuite(IEnumerable<ITest> tests)
			: this() {
			if (tests != null) {
				this.tests.AddRange(tests);
			}
		}

		/// <summary>
		/// Creates a test result for use by the execution context
		/// </summary>
		protected override ITestResult CreateTestResult() {
			return new TestSuiteResult(this) {
				Logger = Logger
			};
		}

		///<inheritdoc/>
		protected override void RunTest(ExecutionContext executionContext) {
			foreach (var test in Tests) {
				var innerContext = CreateInnerContext(executionContext, test);
				test.Run(innerContext);
				((TestSuiteResult)executionContext.Result).AddResult(innerContext.Result);
			}

			ExamineInnerResults(((TestSuiteResult)executionContext.Result));
		}

		/// <summary>
		/// Creates the execution context used by each test in the Tests enumeration
		/// </summary>
		/// <param name="executionContext">The parent execution context</param>
		/// <param name="test">The test this execution context will be for</param>
		protected ExecutionContext CreateInnerContext(ExecutionContext executionContext, ITest test) {
			return new ExecutionContext {
				Instance = executionContext.Instance,
				Test = test
			};
		}

		/// <summary>
		/// Examines the inner tests' results
		/// </summary>
		/// <exception cref="Test.TestFailedException">If any inner tests erred or failed</exception>
		/// <exception cref="Test.TestIgnoredException">If all inner tests were ignored</exception>
		protected void ExamineInnerResults(TestSuiteResult result) {
			if (result.OuterResults.Any(r => r.Status == TestStatus.Error || r.Status == TestStatus.Fail)) {
				throw new TestFailedException("At least one inner test erred or failed");
			}
			if (!result.IsEmpty && result.OuterResults.All(r => r.Status == TestStatus.Ignore)) {
				throw new TestIgnoredException("All inner tests were ignored");
			}
		}

		/// <summary>
		/// Adds a test to the suite. This test can also be another test suite.
		/// </summary>
		public ITestSuite AddTest(ITest test) {
			tests.Add(test);
			return this;
		}

		/// <summary>
		/// Adds a range of tests to the suite
		/// </summary>
		public ITestSuite AddTests(IEnumerable<ITest> listOfTests) {
			tests.AddRange(listOfTests);
			return this;
		}

	}

}
