﻿using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Represents the result of a completed test suite
	/// </summary>
	public class TestSuiteResult : TestResult<TestSuite>, ITestSuiteResult, ILoggable {
		private ILogger logger;
		private readonly IList<ITestResult> results;

		public TestSuiteResult(TestSuite suite) : base(suite) {
			logger = new NullLogger();
			results = new List<ITestResult>();
		}

		/// <summary>
		/// Adds a test result to the collection
		/// </summary>
		/// <param name="result">The test result to add, cannot be null</param>
		public ITestSuiteResult AddResult(ITestResult result) {
			if (result == null) {
				throw new ArgumentNullException("result");
			}

			results.Add(result);
			return this;
		}

		/// <summary>
		/// Gets whether any tests were run
		/// </summary>
		public bool IsEmpty { get { return !Results.Any(); } }
		/// <summary>
		/// Gets the results of all the executed tests ordered chronologically
		/// </summary>
		public IEnumerable<ITestResult> Results { get { return results.OrderBy(result => result.StartTime); } }
		/// <summary>
		/// Gets all tests that passed
		/// </summary>
		public IEnumerable<ITest> PassedTests { get { return Results.Where(result => result.Status == TestStatus.Pass).Select(result => result.Test); } }
		/// <summary>
		/// Gets all tests that failed
		/// </summary>
		public IEnumerable<ITest> FailedTests { get { return Results.Where(result => result.Status == TestStatus.Fail).Select(result => result.Test); } }
		/// <summary>
		/// Gets all tests that were ignored
		/// </summary>
		public IEnumerable<ITest> IgnoredTests { get { return Results.Where(result => result.Status == TestStatus.Ignore).Select(result => result.Test); } }
		/// <summary>
		/// Gets all tests that encountered an error
		/// </summary>
		public IEnumerable<ITest> ErredTests { get { return Results.Where(result => result.Status == TestStatus.Error).Select(result => result.Test); } }
		/// <summary>
		/// Gets all tests run by the suite
		/// </summary>
		public IEnumerable<ITest> AllTests { get { return Results.Select(result => result.Test); } }
		/// <summary>
		/// All errors encountered by all tests contained within the suite
		/// </summary>
		public IEnumerable<Exception> AllErrors {
			get {
				return Results
					.Where(result => result.Status == TestStatus.Error)
					.SelectMany(result => result.Errors)
					.Union(Errors);
			}
		}

		///<inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}
	}
}