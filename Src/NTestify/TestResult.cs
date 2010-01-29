using System;
using System.Collections.Generic;

namespace NTestify {
	/// <summary>
	/// Represents the result of a completed test
	/// </summary>
	public class TestResult : ILoggable {
		/// <summary>
		/// The status of a test. The default is HasNotRun.
		/// </summary>
		public TestStatus Status { get; set; }
		/// <summary>
		/// The time the test started
		/// </summary>
		public long StartTime { get; set; }
		/// <summary>
		/// The time the test ended
		/// </summary>
		public long EndTime { get; set; }
		/// <summary>
		/// How long the test took to execute
		/// </summary>
		public long ExecutionTime { get { return EndTime - StartTime; } }
		/// <summary>
		/// Any message that needs to be relayed to the end-user
		/// </summary>
		public string Message { get; set; }

		private readonly IList<Exception> errors;
		private ILogger logger;

		/// <summary>
		/// All errors that occurred during test execution
		/// </summary>
		public IEnumerable<Exception> Errors { get { return errors; } }

		public TestResult() {
			Status = TestStatus.HasNotRun;
			errors = new List<Exception>();
			logger = new NullLogger();
		}

		/// <summary>
		/// [fluent] Adds an error to the error stack
		/// </summary>
		/// <param name="exception"></param>
		public TestResult AddError(Exception exception) {
			errors.Add(exception);
			return this;
		}
		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}
	}
}