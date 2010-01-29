using System;
using System.Linq;
using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Represents a single testable method, i.e. an instance method that is annotated
	/// with an attribute signifying that is a test
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
		public event Action<ExecutionContext, TestResult> AfterTestRunEvent;
		/// <summary>
		/// Event that fires if a test is ignored
		/// </summary>
		public event Action<ExecutionContext, TestResult> TestIgnoredEvent;
		/// <summary>
		/// Event that fires when a test passes
		/// </summary>
		public event Action<ExecutionContext, TestResult> TestPassedEvent;
		/// <summary>
		/// Event that fires when a test fails
		/// </summary>
		public event Action<ExecutionContext, TestResult> TestFailedEvent;
		/// <summary>
		/// Event that fires when a test errs
		/// </summary>
		public event Action<ExecutionContext, TestResult> TestErredEvent;

		/// <summary>
		/// Creates a new test method
		/// </summary>
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

		/// <summary>
		/// Default AfterTestRunEvent handler
		/// </summary>
		protected virtual void OnAfterTestRun(ExecutionContext executioncontext, TestResult result) {
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
		protected virtual void OnTestPassed(ExecutionContext executioncontext, TestResult result) {
			logger.Debug("Test passed");
		}

		/// <summary>
		/// Default TestFailedEvent handler
		/// </summary>
		protected virtual void OnTestFailed(ExecutionContext executionContext, TestResult result) {
			logger.Debug("Test failed");
		}

		/// <summary>
		/// Default TestIgnoredEvent handler
		/// </summary>
		protected virtual void OnTestIgnored(ExecutionContext executionContext, TestResult result) {
			logger.Debug("Test ignored");
		}

		/// <summary>
		/// Default TestErredEvent handler
		/// </summary>
		protected virtual void OnTestErred(ExecutionContext executionContext, TestResult result) {
			logger.Debug("Test erred");
		}

		/// <inheritdoc/>
		public TestResult Run(ExecutionContext executionContext) {
			executionContext.Test = this;

			var result = new TestResult {
				Status = TestStatus.Running,
				StartTime = DateTime.Now.Ticks
			};

			if (VerifyMethod(method)) {
				try {
					BeforeTestRunEvent.Invoke(executionContext);
					RunTestMethod(method, executionContext, result);
					result.Status = TestStatus.Pass;
				} catch (TargetInvocationException invocationException) {
					//this exception is not what we're interested in, but it's what gets thrown from
					//the reflected method
					if (invocationException.InnerException is TestAssertionException) {
						result.Status = TestStatus.Fail;
						result.Message = invocationException.InnerException.Message;
					} else if (invocationException.InnerException is TestIgnoredException) {
						result.Status = TestStatus.Ignore;
						result.Message = invocationException.InnerException.Message;
					} else {
						HandleError(invocationException.InnerException ?? invocationException, executionContext, result);
					}
				} catch (Exception exception) {
					HandleError(exception.InnerException ?? exception, executionContext, result);
				} finally {
					AfterTestRunEvent.Invoke(executionContext, result);
				}
			} else {
				result.Status = TestStatus.Error;
				result.Message = "Method is invalid";
			}

			result.EndTime = DateTime.Now.Ticks;
			InvokeStatusEvent(executionContext, result);
			return result;
		}

		private void InvokeStatusEvent(ExecutionContext executionContext, TestResult result) {
			switch (result.Status) {
				case TestStatus.Pass:
					TestPassedEvent.Invoke(executionContext, result);
					break;
				case TestStatus.Fail:
					TestFailedEvent.Invoke(executionContext, result);
					break;
				case TestStatus.Error:
					TestErredEvent.Invoke(executionContext, result);
					break;
				case TestStatus.Ignore:
					TestIgnoredEvent.Invoke(executionContext, result);
					break;
			}
		}

		/// <summary>
		/// Invokes the test method
		/// </summary>
		/// <param name="methodInfo">The test method to invoke</param>
		/// <param name="executionContext">The current test execution context</param>
		/// <param name="result">The test result</param>
		protected virtual void RunTestMethod(MethodInfo methodInfo, ExecutionContext executionContext, TestResult result) {
			method.Invoke(executionContext.Instance, new object[] { });
		}

		/// <summary>
		/// Handles an erred test (i.e. sets the test result status)
		/// </summary>
		/// <param name="exception">The raised exception</param>
		/// <param name="executionContext">The current test execution context</param>
		/// <param name="result">The test result</param>
		protected void HandleError(Exception exception, ExecutionContext executionContext, TestResult result) {
			result.AddError(exception);
			result.Status = TestStatus.Error;
		}

		/// <summary>
		/// Verifies that the method is a valid, testable method
		/// </summary>
		/// <param name="methodToExecute">The test method that will be executed</param>
		protected virtual bool VerifyMethod(MethodInfo methodToExecute) {
			return
				!methodToExecute.IsConstructor && !methodToExecute.IsAbstract &&
				!methodToExecute.IsStatic && !methodToExecute.GetParameters().Any();
		}

		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}
	}
}
