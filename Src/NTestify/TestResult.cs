using System;
using System.Collections.Generic;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Base class for strongly typed test results
	/// </summary>
	/// <typeparam name="TTest">The type of test this result corresponds to</typeparam>
	public class TestResult<TTest> : ITestResult, ILoggable<TestResult<TTest>> where TTest : ITest {
		private readonly TTest test;
		private readonly List<Exception> errors;

		public TestResult(TTest test) {
			this.test = test;
			errors = new List<Exception>();
			Status = TestStatus.HasNotRun;
		}

		/// <summary>
		/// The status of a test. The default is HasNotRun.
		/// </summary>
		public TestStatus Status { get; set; }
		/// <summary>
		/// The time the test started
		/// </summary>
		public DateTime StartTime { get; set; }
		/// <summary>
		/// The time the test ended
		/// </summary>
		public DateTime EndTime { get; set; }
		/// <summary>
		/// How long the test took to execute
		/// </summary>
		public long ExecutionTime { get { return EndTime.Ticks - StartTime.Ticks; } }
		/// <summary>
		/// How long the test took to execute in seconds
		/// </summary>
		public decimal ExecutionTimeInSeconds { get { return ExecutionTime / 10000000m; } }
		/// <summary>
		/// Any message that needs to be relayed to the end-user
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// All errors that occurred during test execution
		/// </summary>
		public IEnumerable<Exception> Errors { get { return errors; } }

		/// <summary>
		/// [fluent] Adds an error to the error stack
		/// </summary>
		/// <param name="exception">The exception that caused the error. Cannot be null.</param>
		public ITestResult AddError(Exception exception) {
			if (exception == null) {
				throw new ArgumentNullException("exception");
			}

			errors.Add(exception);
			return this;
		}

		/// <summary>
		/// The test that was executed
		/// </summary>
		public ITest Test { get { return test; } }

		///<inheritdoc/>
		public TestResult<TTest> SetLogger(ILogger logger){
			Logger = logger;
			return this;
		}

		/// <summary>
		/// Gets the logger for this result
		/// </summary>
		protected ILogger Logger { get; private set; }

		///<inheritdoc/>
		public override string ToString() {
			return string.Format(
				"{0} ({1} total tests): Status = {2}, ExecutionTime = {3}", 
				GetType().FullName,
				1, 
				Status, 
				ExecutionTimeInSeconds
			);
		}
	}
}