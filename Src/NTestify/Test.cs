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
		private ITestConfigurator configurator;

		public ITestConfigurator Configurator {
			get { return configurator ?? (configurator = new NullConfigurator()); }
			set { configurator = value; }
		}

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
				executionContext.ThrownException = exception;
			}

			try {
				RunFilters<PostTestFilter>(executionContext);
			} catch (Exception filterException) {
				Error(executionContext, filterException);
			}

			if (executionContext.Result.Status == TestStatus.Running) {
				SetResultStatus(executionContext);
			}

			executionContext.Result.EndTime = DateTime.Now;

			OnAfterRun.Invoke(executionContext);
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
				Error(executionContext, exception);
			} else if (exception is TestIgnoredException) {
				Ignore(executionContext, exception.Message);
			} else if (exception != null) {
				Error(executionContext, exception.GetBaseException());
			} else {
				Pass(executionContext); //this shouldn't ever happen, since if a test passes it gets set immediately after RunTest()
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
			OnIgnore.Invoke(executionContext);
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
			}

			executionContext.Result.Message = (exception != null) ? exception.Message : "An error occurred during execution of the test";
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

	}
}