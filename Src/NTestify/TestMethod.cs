using System;
using System.Linq;
using System.Reflection;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Represents a single testable method, i.e. an instance method that is annotated
	/// with an attribute signifying that it is a test
	/// </summary>
	public class TestMethod : ITest, ILoggable {
		private readonly MethodInfo method;
		private ILogger logger;

		/// <summary>
		/// Event that fires before a test runs
		/// </summary>
		public event Action<ExecutionContext> BeforeTestRunEvent;
		/// <summary>
		/// Event that fires after a test runs
		/// </summary>
		public event Action<ExecutionContext> AfterTestRunEvent;
		/// <summary>
		/// Event that fires if a test is ignored
		/// </summary>
		public event Action<ExecutionContext> TestIgnoredEvent;
		/// <summary>
		/// Event that fires when a test passes
		/// </summary>
		public event Action<ExecutionContext> TestPassedEvent;
		/// <summary>
		/// Event that fires when a test fails
		/// </summary>
		public event Action<ExecutionContext> TestFailedEvent;
		/// <summary>
		/// Event that fires when a test errs
		/// </summary>
		public event Action<ExecutionContext> TestErredEvent;

		/// <param name="method">The test method to run. Cannot be null.</param>
		public TestMethod(MethodInfo method) {
			if (method == null) {
				throw new ArgumentNullException("method");
			}

			logger = new NullLogger();

			this.method = method;

			BeforeTestRunEvent += OnBeforeTestRun;
			AfterTestRunEvent += OnAfterTestRun;
			TestIgnoredEvent += OnTestIgnored;
			TestFailedEvent += OnTestFailed;
			TestErredEvent += OnTestErred;
			TestPassedEvent += OnTestPassed;
		}

		#region Default event handlers
		/// <summary>
		/// Default AfterTestRunEvent handler
		/// </summary>
		protected virtual void OnAfterTestRun(ExecutionContext executioncontext) {
			logger.Debug("After test run");
		}

		/// <summary>
		/// Default BeforeTestRunEvent handler
		/// </summary>
		protected virtual void OnBeforeTestRun(ExecutionContext executionContext) {
			logger.Debug("Before test run");
		}

		/// <summary>
		/// Default TestPassedEvent handler
		/// </summary>
		protected virtual void OnTestPassed(ExecutionContext executioncontext) {
			logger.Debug("Test passed");
		}

		/// <summary>
		/// Default TestFailedEvent handler
		/// </summary>
		protected virtual void OnTestFailed(ExecutionContext executionContext) {
			logger.Debug("Test failed");
		}

		/// <summary>
		/// Default TestIgnoredEvent handler
		/// </summary>
		protected virtual void OnTestIgnored(ExecutionContext executionContext) {
			logger.Debug("Test ignored");
		}

		/// <summary>
		/// Default TestErredEvent handler
		/// </summary>
		protected virtual void OnTestErred(ExecutionContext executionContext) {
			logger.Debug("Test erred");
		}
		#endregion

		/// <inheritdoc/>
		public void Run(ExecutionContext executionContext) {
			executionContext.Test = this;

			BeforeTestRunEvent.Invoke(executionContext);
			executionContext.Result = CreateTestResult();
			executionContext.Result.StartTime = DateTime.Now;
			executionContext.Result.Status = TestStatus.Running;
			try {
				
				RunMethodFilters(executionContext);
				VerifyMethod(executionContext);
				if (executionContext.Result.Status == TestStatus.Running) {
					//only run the test if the filters didn't modify the test status
					RunTestMethod(method, executionContext);
					executionContext.Result.Status = TestStatus.Pass;
				}
			} catch (TargetInvocationException invocationException) {
				//this exception is not what we're interested in, but it's what gets thrown from
				//the reflected method
				HandleInvocationException(invocationException, executionContext);
			} catch (Exception exception) {
				HandleError(exception.InnerException ?? exception, executionContext);
			}

			executionContext.Result.EndTime = DateTime.Now;
			InvokeStatusEvent(executionContext);
			AfterTestRunEvent.Invoke(executionContext);
		}

		/// <summary>
		/// Runs the method filters
		/// </summary>
		protected virtual void RunMethodFilters(ExecutionContext executionContext) {
			var filters = method.GetAttributes<TestifyAttribute>();
			if (filters.Any(attribute => attribute is IgnoreAttribute)) {
				//if the test should be ignored, then we bail immediately without executing any other filters
				IgnoreTest(executionContext.Result, ((IgnoreAttribute)filters.First(attribute => attribute is IgnoreAttribute)).Reason);
				return;
			}

			foreach (var filter in filters) {
				try {
					filter.Execute(executionContext);
				} catch (Exception exception) {
					OnFilterError(exception, filter, executionContext);
					break;
				}
			}
		}

		/// <summary>
		/// Called when a filter encounters an error
		/// </summary>
		/// <param name="exception">The raised exception</param>
		/// <param name="filter">The filter that encountered the error</param>
		/// <param name="executionContext">The current test execution context</param>
		protected virtual void OnFilterError(Exception exception, object filter, ExecutionContext executionContext) {
			executionContext.Result.Status = TestStatus.Error;
			executionContext.Result.Message = string.Format("Encountered an error while trying to run method filter \"{0}\"", filter.GetType().FullName);
			executionContext.Result.AddError(exception);
		}

		/// <summary>
		/// Ignores the test and updates the test result accordingly
		/// </summary>
		/// <param name="result">The test method result</param>
		/// <param name="reason">The reason the test was ignored</param>
		protected void IgnoreTest(ITestResult result, string reason) {
			result.Status = TestStatus.Ignore;
			result.Message = reason;
		}

		/// <summary>
		/// Handles invocation exceptions. This is relevant because we're invoking the
		/// test method via reflection, and any exception thrown from there raises a
		/// TargetInvocationException. This method examines the InnerException (if any)
		/// and sets the result accordingly.
		/// </summary>
		private void HandleInvocationException(TargetInvocationException invocationException, ExecutionContext executionContext) {
			if (invocationException.InnerException is TestAssertionException) {
				//an assertion failed
				executionContext.Result.Status = TestStatus.Fail;
				executionContext.Result.Message = invocationException.InnerException.Message;
			} else {
				HandleError(invocationException.InnerException ?? invocationException, executionContext);
			}
		}

		/// <summary>
		/// Creates the test result that will be attached to the execution
		/// context after the test has been run
		/// </summary>
		protected virtual TestMethodResult CreateTestResult() {
			var result = new TestMethodResult(this);
			result.SetLogger(logger);
			return result;
		}

		/// <summary>
		/// Fires the appropriate events based on the status of the test result
		/// </summary>
		private void InvokeStatusEvent(ExecutionContext executionContext) {
			switch (executionContext.Result.Status) {
				case TestStatus.Pass:
					TestPassedEvent.Invoke(executionContext);
					break;
				case TestStatus.Fail:
					TestFailedEvent.Invoke(executionContext);
					break;
				case TestStatus.Error:
					TestErredEvent.Invoke(executionContext);
					break;
				case TestStatus.Ignore:
					TestIgnoredEvent.Invoke(executionContext);
					break;
			}
		}

		/// <summary>
		/// Invokes the test method
		/// </summary>
		/// <param name="methodInfo">The test method to invoke</param>
		/// <param name="executionContext">The current test execution context</param>
		protected virtual void RunTestMethod(MethodInfo methodInfo, ExecutionContext executionContext) {
			method.Invoke(executionContext.Instance, new object[] { });
		}

		/// <summary>
		/// Handles an erred test (i.e. sets the test result status)
		/// </summary>
		/// <param name="exception">The raised exception</param>
		/// <param name="executionContext">The current test execution context</param>
		protected void HandleError(Exception exception, ExecutionContext executionContext) {
			executionContext.Result.AddError(exception);
			executionContext.Result.Status = TestStatus.Error;
		}

		/// <summary>
		/// Verifies that the test method is a valid, testable method
		/// </summary>
		protected void VerifyMethod(ExecutionContext executionContext) {
			if (!method.IsRunnable()) {
				executionContext.Result.Status = TestStatus.Error;
				executionContext.Result.Message = "The test method is invalid";
			}
		}

		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}
	}
}
