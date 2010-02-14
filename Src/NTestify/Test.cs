using System;
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
		internal sealed class TestErredException : Exception {
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
		internal sealed class TestFailedException : Exception {
			public TestFailedException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception that indicates that a test was ignored
		/// </summary>
		internal sealed class TestIgnoredException : Exception {
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

		private ITestConfigurator configurator;

		public ITestConfigurator Configurator {
			get { return configurator ?? (configurator = new NullConfigurator()); }
			set { configurator = value; }
		}

		protected Test() {
			Logger = new NullLogger();
			SetDefaultEventHandlers();
		}

		///<inheritdoc/>
		public void Run(ExecutionContext executionContext) {
			InitializeContext(executionContext);
			Configurator.Configure(executionContext.Test);
			OnBeforeRun.Invoke(executionContext);
			executionContext.Result.Status = TestStatus.Running;

			try {
				RunFilters<PreTestFilter>(executionContext);
				RunTest(executionContext);
				Pass(executionContext);
			} catch (Exception exception) {
				var erredException = exception as TestErredException;
				executionContext.ThrownException = (erredException != null && erredException.CauseError != null) ? erredException.CauseError : exception;
			}

			try {
				RunFilters<PostTestFilter>(executionContext);
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
		protected virtual void RunFilters<TFilter>(ExecutionContext executionContext) where TFilter : TestifyAttribute { }

		/// <summary>
		/// Sets the result status
		/// </summary>
		private void SetResultStatus(ExecutionContext executionContext) {
			var exception = executionContext.ThrownException;

			if (exception is TestFailedException) {
				Fail(executionContext, exception.Message);
			} else if (exception is TestErredException) {
				Error(executionContext, ((TestErredException)exception).CauseError ?? exception);
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

	}
}