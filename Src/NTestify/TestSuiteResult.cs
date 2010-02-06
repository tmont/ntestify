using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NTestify {
	/// <summary>
	/// Represents the result of a completed test suite
	/// </summary>
	[DebuggerDisplay("Status = {Status}, Execution Time = {ExecutionTimeInSeconds}s, Test = {Test.Name}")]
	public class TestSuiteResult : TestResult<TestSuite> {
		private readonly IList<ITestResult> results;

		public TestSuiteResult(TestSuite suite)
			: base(suite) {
			results = new List<ITestResult>();
		}

		/// <summary>
		/// Adds a test result to the collection
		/// </summary>
		///<exception cref="ArgumentNullException"/>
		public TestSuiteResult AddResult(ITestResult result) {
			if (result == null) {
				throw new ArgumentNullException("result");
			}

			results.Add(result);
			return this;
		}

		#region Properties
		/// <summary>
		/// Gets whether or not any tests were run
		/// </summary>
		public bool IsEmpty { get { return !OuterResults.Any(); } }
		/// <summary>
		/// Gets the results of all the executed tests ordered chronologically (this list is not flattened)
		/// </summary>
		public IEnumerable<ITestResult> OuterResults { get { return results.OrderBy(result => result.StartTime); } }
		/// <summary>
		/// Gets the number of outer tests
		/// </summary>
		public int OuterCount { get { return OuterTests.Count(); } }
		/// <summary>
		/// Gets all inner tests that passed
		/// </summary>
		public IEnumerable<ITest> InnerPassedTests { get { return InnerResults.Where(result => result.Status == TestStatus.Pass).Select(result => result.Test); } }
		/// <summary>
		/// Gets all inner tests that failed
		/// </summary>
		public IEnumerable<ITest> InnerFailedTests { get { return InnerResults.Where(result => result.Status == TestStatus.Fail).Select(result => result.Test); } }
		/// <summary>
		/// Gets all inner tests that were ignored
		/// </summary>
		public IEnumerable<ITest> InnerIgnoredTests { get { return InnerResults.Where(result => result.Status == TestStatus.Ignore).Select(result => result.Test); } }
		/// <summary>
		/// Gets all outer tests that passed
		/// </summary>
		public IEnumerable<ITest> OuterPassedTests { get { return OuterResults.Where(result => result.Status == TestStatus.Pass).Select(result => result.Test); } }
		/// <summary>
		/// Gets all outer tests that encountered an error
		/// </summary>
		public IEnumerable<ITest> OuterErredTests { get { return OuterResults.Where(result => result.Status == TestStatus.Error).Select(result => result.Test); } }
		/// <summary>
		/// Gets all outer tests that failed
		/// </summary>
		public IEnumerable<ITest> OuterFailedTests { get { return OuterResults.Where(result => result.Status == TestStatus.Fail).Select(result => result.Test); } }
		/// <summary>
		/// Gets all outer tests that were ignored
		/// </summary>
		public IEnumerable<ITest> OuterIgnoredTests { get { return OuterResults.Where(result => result.Status == TestStatus.Ignore).Select(result => result.Test); } }
		/// <summary>
		/// Gets all inner tests that encountered an error
		/// </summary>
		public IEnumerable<ITest> InnerErredTests { get { return InnerResults.Where(result => result.Status == TestStatus.Error).Select(result => result.Test); } }
		/// <summary>
		/// Gets all tests run by the suite
		/// </summary>
		public IEnumerable<ITest> OuterTests { get { return OuterResults.Select(result => result.Test); } }
		/// <summary>
		/// Gets all outer errors that occurred during test execution
		/// </summary>
		public IEnumerable<Exception> OuterErrors {
			get {
				return OuterResults
					.SelectMany(result => result.Errors)
					.Union(Errors);
			}
		}
		/// <summary>
		/// Gets all inner errors that occurred during test execution
		/// </summary>
		public IEnumerable<Exception> InnerErrors { get { return InnerResults.SelectMany(result => result.Errors).Union(Errors); } }
		/// <summary>
		/// Gets a flattened list of all inner test results
		/// </summary>
		public IEnumerable<ITestResult> InnerResults {
			get {
				var returnResults = new List<ITestResult>();
				foreach (var result in OuterResults) {
					if (result is TestSuiteResult) {
						returnResults.AddRange(((TestSuiteResult)result).InnerResults);
					} else {
						returnResults.Add(result);
					}
				}

				return returnResults;
			}
		}
		/// <summary>
		/// Gets a flattened list of all inner tests
		/// </summary>
		public IEnumerable<ITest> InnerTests { get { return InnerResults.Select(result => result.Test); } }
		/// <summary>
		/// Gets the number of tests contained within this result
		/// </summary>
		public int InnerCount { get { return InnerTests.Count(); } }
		#endregion

		///<inheritdoc/>
		public override string ToString() {
			return string.Format(
				"{0} ({1} inner tests, {2} outer tests): Status = {3}, ExecutionTime = {4}",
				GetType().FullName,
				InnerCount,
				OuterCount,
				Status,
				ExecutionTimeInSeconds
			);
		}
	}
}