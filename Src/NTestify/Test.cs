using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Configuration;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Base class for all built-in, runnable tests
	/// </summary>
	public abstract class Test : ITest, ILoggable {

		#region Inner exceptions
		/// <summary>
		/// Exception that indicates that a test encountered an error
		/// </summary>
		protected internal sealed class TestErredException : Exception {
			/// <param name="error">The exception that caused the error</param>
			/// <param name="reason">The reason the test erred</param>
			public TestErredException(Exception error, string reason)
				: base(reason) {
				CauseError = error;
			}

			/// <summary>
			/// The exception that caused the error
			/// </summary>
			public Exception CauseError { get; private set; }
		}

		/// <summary>
		/// Exception that indicates that a test failed
		/// </summary>
		protected internal sealed class TestFailedException : Exception {
			public TestFailedException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception that indicates that a test was ignored
		/// </summary>
		protected internal sealed class TestIgnoredException : Exception {
			public TestIgnoredException(string reason) : base(reason) { }
		}
		#endregion

		#region Event stuff
		public event Action<ExecutionContext> OnBeforeRun;
		public event Action<ExecutionContext> OnAfterRun;
		public event Action<ExecutionContext> OnIgnore;
		public event Action<ExecutionContext> OnPass;
		public event Action<ExecutionContext> OnFail;
		public event Action<ExecutionContext> OnError;

		/// <summary>
		/// Sets the default event handlers to empty lambda expressions, to eliminate null references
		/// </summary>
		private void SetDefaultEventHandlers() {
			OnBeforeRun += context => { };
			OnAfterRun += context => { };
			OnIgnore += context => { };
			OnPass += context => { };
			OnFail += context => { };
			OnError += context => { };
		}
		#endregion

		/// <summary>
		/// Gets or sets the exception that is expected to be thrown during execution
		/// of the test. If no exception is expected to be thrown, this value should
		/// be null.
		/// </summary>
		public Type ExpectedException { get; set; }

		/// <summary>
		/// Gets or sets the message of the expected exception
		/// </summary>
		public string ExpectedExceptionMessage { get; set; }

		/// <summary>
		/// Uses the given configurator to configure the test
		/// </summary>
		public ITest Configure(ITestConfigurator configurator) {
			configurator = configurator ?? new NullConfigurator();
			configurator.Configure(this);
			return this;
		}

		protected Test() {
			Logger = new NullLogger();
			SetDefaultEventHandlers();
		}

		/// <summary>
		/// Runs the test, and sets the ExecutionContext's Result property
		/// with the result of the test
		/// </summary>
		/// <param name="executionContext">The current test execution context</param>
		public void Run(ExecutionContext executionContext) {
			Initialize(executionContext);
			OnBeforeRun.Invoke(executionContext);
			executionContext.Result.Status = TestStatus.Running;

			try {
				RunPreTestFilters(executionContext);
				RunTest(executionContext);
				Pass(executionContext);
			} catch (Exception exception) {
				executionContext.ThrownException = exception;
			}

			HandleThrownException(executionContext);

			try {
				RunPostTestFilters(executionContext);
			} catch (Exception filterException) {
				Error(executionContext, filterException);
			}

			if (executionContext.Result.Status == TestStatus.Running) {
				SetResultStatus(executionContext);
			}

			InvokeStatusEvent(executionContext);
			executionContext.Result.EndTime = DateTime.Now;
			OnAfterRun.Invoke(executionContext);
		}

		/// <summary>
		/// Does the expected exception stuff
		/// </summary>
		protected void HandleThrownException(ExecutionContext executionContext) {
			if (ExpectedException == null) {
				return;
			}

			if (executionContext.ThrownException == null) {
				Fail(executionContext, string.Format("Expected exception of type {0} was never thrown.", ExpectedException.GetFriendlyName()));
				return;
			}

			var temp = executionContext.ThrownException.GetBaseException();
			var thrownException = (temp is TestErredException && ((TestErredException)temp).CauseError != null) ? ((TestErredException)temp).CauseError : temp;

			if (thrownException.GetType() != ExpectedException) {
				Fail(executionContext, string.Format(
					"Expected exception of type {0}, but exception of type {1} was thrown.",
					ExpectedException.GetFriendlyName(),
					thrownException.GetType().GetFriendlyName()
				));
			} else if (!string.IsNullOrEmpty(ExpectedExceptionMessage) && thrownException.Message != ExpectedExceptionMessage) {
				Fail(executionContext, string.Format(
					"Expected exception message did not match actual exception message.\nExpected: {0}\nActual:   {1}",
					ExpectedExceptionMessage,
					thrownException.Message
				));
			} else {
				//the test threw the correct exception with the correct message, so the result should be pass
				Pass(executionContext);
			}
		}

		/// <summary>
		/// Invokes the proper status event based on the execution context's result status
		/// </summary>
		/// <param name="executionContext"></param>
		protected void InvokeStatusEvent(ExecutionContext executionContext) {
			switch (executionContext.Result.Status) {
				case TestStatus.Pass:
					OnPass.Invoke(executionContext);
					break;
				case TestStatus.Fail:
					OnFail.Invoke(executionContext);
					break;
				case TestStatus.Error:
					OnError.Invoke(executionContext);
					break;
				case TestStatus.Ignore:
					OnIgnore.Invoke(executionContext);
					break;
			}
		}

		/// <summary>
		/// Runs any filters attached to the test after the test is run
		/// </summary>
		protected virtual void RunPostTestFilters(ExecutionContext executionContext) { }

		/// <summary>
		/// Runs any filters attached to the test before the test is run
		/// </summary>
		protected virtual void RunPreTestFilters(ExecutionContext executionContext) { }

		/// <summary>
		/// Sets the result status
		/// </summary>
		private void SetResultStatus(ExecutionContext executionContext) {
			var exception = executionContext.ThrownException;

			if (exception is TestFailedException) {
				Fail(executionContext, exception.Message);
			} else if (exception is TestErredException) {
				Error(executionContext, exception);
			} else if (exception is TestIgnoredException) {
				Ignore(executionContext, exception.Message);
			} else if (exception != null) {
				Error(executionContext, exception.GetBaseException());
			} else {
				Pass(executionContext); //this should only happen if a post test filter sets the status to Running or something
			}
		}

		/// <summary>
		/// Sets the initial state of the execution context and its
		/// encapsulated result
		/// </summary>
		private void Initialize(ExecutionContext executionContext) {
			executionContext.Test = this;
			executionContext.Result = CreateTestResult();
			executionContext.Result.StartTime = DateTime.Now;
			InitializeContext(executionContext);
		}

		/// <summary>
		/// When overridden in a derived class, initializes the execution context
		/// for pre test filters and before configuration
		/// </summary>
		/// <param name="executionContext"></param>
		protected virtual void InitializeContext(ExecutionContext executionContext) { }

		/// <summary>
		/// Creates the test result that will be attached to the execution context
		/// </summary>
		protected abstract ITestResult CreateTestResult();

		/// <summary>
		/// Runs the test. This method should do the actual heavy lifting.
		/// It should use the internal exceptions to signify the result of
		/// the test, rather than setting the result property itself.
		/// </summary>
		protected abstract void RunTest(ExecutionContext executionContext);

		/// <summary>
		/// Gets or sets the name of the test
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the test
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the category for the test
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the logger associated with this test
		/// </summary>
		public ILogger Logger { get; set; }

		/// <summary>
		/// Marks the test result ignored
		/// </summary>
		protected void Ignore(ExecutionContext executionContext, string reason) {
			executionContext.Result.Status = TestStatus.Ignore;
			executionContext.Result.Message = reason;
		}

		/// <summary>
		/// Marks the test result erred
		/// </summary>
		protected void Error(ExecutionContext executionContext, Exception exception) {
			executionContext.Result.Status = TestStatus.Error;
			var erredException = exception as TestErredException;

			if (erredException != null) {
				if (erredException.CauseError != null) {
					executionContext.Result.AddError(erredException.CauseError);
				}
			} else {
				executionContext.Result.AddError(exception);
			}

			executionContext.Result.Message = (exception != null) ? exception.Message : "An error occurred during execution of the test";
		}

		/// <summary>
		/// Marks the test result failed
		/// </summary>
		protected void Fail(ExecutionContext executionContext, string message) {
			executionContext.Result.Status = TestStatus.Fail;
			executionContext.Result.Message = message;
		}

		/// <summary>
		/// Marks the test result passed
		/// </summary>
		protected void Pass(ExecutionContext executionContext) {
			executionContext.Result.Status = TestStatus.Pass;
		}

		/// <summary>
		/// Runs all filters ordered by their Order property
		/// </summary>
		protected void RunFiltersInOrder(IEnumerable<TestFilter> filters, ExecutionContext executionContext){
			foreach (var filter in filters.OrderBy(f => f.Order)) {
				try {
					filter.Execute(executionContext);
				} catch (Exception exception) {
					OnFilterError(exception, filter, executionContext);
				}
			}
		}

		/// <summary>
		/// Called when a filter encounters an error
		/// </summary>
		/// <param name="exception">The raised exception</param>
		/// <param name="filter">The filter that encountered the error</param>
		/// <param name="executionContext">The current test execution context</param>
		/// <exception cref="TestErredException"/>
		protected virtual void OnFilterError(Exception exception, TestFilter filter, ExecutionContext executionContext) {
			var message = string.Format("Encountered an error while trying to run method filter of type \"{0}\"", filter.GetType().GetFriendlyName());
			throw new TestErredException(exception, message);
		}

	}
}