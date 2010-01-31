using System;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Base class for all built-in, runnable tests
	/// </summary>
	public abstract class Test : ITest, ILoggable<Test> {

		#region Inner exceptions
		/// <summary>
		/// Exception that indicates that a test encountered an error
		/// </summary>
		internal class TestErredException : Exception {
			/// <param name="error">The exception that caused the error</param>
			/// <param name="reason">The reason the test erred</param>
			public TestErredException(Exception error, string reason)
				: base(reason) {
				CauseError = error;
			}

			/// <summary>
			/// The exception that caused the error
			/// </summary>
			public Exception CauseError { get; protected set; }
		}

		/// <summary>
		/// Exception that indicates that a test failed
		/// </summary>
		internal class TestFailedException : Exception {
			public TestFailedException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception that indicates that a test was ignored
		/// </summary>
		internal class TestIgnoredException : Exception {
			public TestIgnoredException(string reason) : base(reason) { }
		}
		#endregion

		#region Event stuff
		/// <summary>
		/// Event that fires before a test runs
		/// </summary>
		public event Action<ExecutionContext> OnBeforeRun;
		/// <summary>
		/// Event that fires after a test runs
		/// </summary>
		public event Action<ExecutionContext> OnAfterRun;
		/// <summary>
		/// Event that fires if a test is ignored
		/// </summary>
		public event Action<ExecutionContext> OnIgnore;
		/// <summary>
		/// Event that fires when a test passes
		/// </summary>
		public event Action<ExecutionContext> OnPass;
		/// <summary>
		/// Event that fires when a test fails
		/// </summary>
		public event Action<ExecutionContext> OnFail;
		/// <summary>
		/// Event that fires when a test errs
		/// </summary>
		public event Action<ExecutionContext> OnError;

		/// <summary>
		/// Sets the default event handlers, which just log a boring message
		/// </summary>
		private void SetDefaultEventHandlers() {
			OnBeforeRun += context => Logger.Debug("Test [" + Name + "] is starting at " + context.Result.StartTime.ToString("yyyy-MM-dd hh:mm:ss.fff"));
			OnAfterRun += context => Logger.Debug("Test [" + Name + "] finished at " + context.Result.EndTime.ToString("yyyy-MM-dd hh:mm:ss.fff") + " (" + context.Result.ExecutionTimeInSeconds + " seconds)");
			OnIgnore += context => Logger.Warn("Test [" + Name + "] was ignored");
			OnPass += context => Logger.Info("Test [" + Name + "] passed");
			OnFail += context => Logger.Error("Test [" + Name + "] failed");
			OnError += context => Logger.Error("Test [" + Name + "] erred");
		}
		#endregion

		protected Test() {
			Logger = new NullLogger();
			SetDefaultEventHandlers();
		}

		///<inheritdoc/>
		public void Run(ExecutionContext executionContext) {
			InitializeContext(executionContext);

			OnBeforeRun.Invoke(executionContext);

			executionContext.Result.Status = TestStatus.Running;
			try {
				RunFilters(executionContext);
				RunTest(executionContext);
				Pass(executionContext);
			} catch (Exception exception) {
				HandleException(exception, executionContext);
			}
			executionContext.Result.EndTime = DateTime.Now;

			OnAfterRun.Invoke(executionContext);
		}

		/// <summary>
		/// Handles an exception thrown by a test
		/// </summary>
		private void HandleException(Exception exception, ExecutionContext executionContext){
			if (exception is TestFailedException) {
				Fail(executionContext, exception.Message);
			} else if (exception is TestErredException) {
				Error(executionContext, exception);
			} else if (exception is TestIgnoredException) {
				Ignore(executionContext, exception.Message);
			} else {
				Error(executionContext, exception.GetInnermostException());
			}
		}

		/// <summary>
		/// Sets the initial state of the execution context and its
		/// encapsulated result
		/// </summary>
		private void InitializeContext(ExecutionContext executionContext) {
			executionContext.Test = this;
			executionContext.Result = CreateTestResult();
			executionContext.Result.StartTime = DateTime.Now;
		}

		/// <summary>
		/// Creates the test result that will be attached to the execution
		/// context after the test has been run
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
		/// Gets the logger associated with this test
		/// </summary>
		protected ILogger Logger { get; private set; }

		///<inheritdoc/>
		public Test SetLogger(ILogger logger) {
			Logger = logger;
			return this;
		}

		/// <summary>
		/// Marks the test result ignored
		/// </summary>
		protected void Ignore(ExecutionContext executionContext, string reason) {
			executionContext.Result.Status = TestStatus.Ignore;
			executionContext.Result.Message = reason;
			OnIgnore.Invoke(executionContext);
		}

		/// <summary>
		/// Marks the test result erred
		/// </summary>
		protected void Error(ExecutionContext executionContext, Exception exception) {
			executionContext.Result.Status = TestStatus.Error;
			var erredException = exception as TestErredException;
			
			if (erredException != null) {
				var cause = ((TestErredException)exception).CauseError;
				if (cause != null) {
					executionContext.Result.AddError(cause);
				}
			}

			executionContext.Result.Message = exception.Message;
			OnError.Invoke(executionContext);
		}

		/// <summary>
		/// Marks the test result failed
		/// </summary>
		protected void Fail(ExecutionContext executionContext, string message) {
			executionContext.Result.Status = TestStatus.Fail;
			executionContext.Result.Message = message;
			OnFail.Invoke(executionContext);
		}

		/// <summary>
		/// Marks the test result passed
		/// </summary>
		/// <param name="executionContext"></param>
		protected void Pass(ExecutionContext executionContext) {
			executionContext.Result.Status = TestStatus.Pass;
			OnPass.Invoke(executionContext);
		}

		/// <summary>
		/// Runs any filters that are attached to the test 
		/// </summary>
		protected virtual void RunFilters(ExecutionContext executionContext) { }
	}
}