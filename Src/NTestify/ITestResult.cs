using System;
using System.Collections.Generic;
using System.IO;
using NTestify.Configuration;

namespace NTestify {
	/// <summary>
	/// Represents the result of an executed test
	/// </summary>
	public interface ITestResult {
		/// <summary>
		/// The status of a test. The default is HasNotRun.
		/// </summary>
		TestStatus Status { get; set; }
		/// <summary>
		/// The time the test started
		/// </summary>
		DateTime StartTime { get; set; }
		/// <summary>
		/// The time the test ended
		/// </summary>
		DateTime EndTime { get; set; }
		/// <summary>
		/// How long the test took to execute
		/// </summary>
		long ExecutionTime { get; }
		/// <summary>
		/// How long the test took to execute in seconds
		/// </summary>
		decimal ExecutionTimeInSeconds { get; }
		/// <summary>
		/// Any message that needs to be relayed to the end-user
		/// </summary>
		string Message { get; set; }
		/// <summary>
		/// All errors that occurred during test execution
		/// </summary>
		IEnumerable<Exception> Errors { get; }

		/// <summary>
		/// Adds an error to the error stack
		/// </summary>
		/// <param name="exception"></param>
		ITestResult AddError(Exception exception);

		/// <summary>
		/// The test that was executed
		/// </summary>
		ITest Test { get; }
	}
}