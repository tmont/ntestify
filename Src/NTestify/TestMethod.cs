using System;
using System.Linq;
using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Represents a single testable method, i.e. an instance method that is annotated
	/// with an attribute signifying that it is a test
	/// </summary>
	public class TestMethod : Test {
		private readonly MethodInfo method;

		/// <param name="method">The test method to run. Cannot be null.</param>
		public TestMethod(MethodInfo method) {
			if (method == null) {
				throw new ArgumentNullException("method");
			}

			this.method = method;
		}

		/// <inheritdoc/>
		protected override void RunTest(ExecutionContext executionContext) {
			VerifyMethod(executionContext);

			try {
				RunTestMethod(executionContext);
			} catch (Exception exception) {
				HandleException(exception, executionContext);
			}
		}

		/// <summary>
		/// Finds and executes each of the method filters
		/// </summary>
		/// <exception cref="Test.TestIgnoredException">If the method is annotated with [Ignore]</exception>
		protected override void RunFilters(ExecutionContext executionContext) {
			var filters = method.GetAttributes<TestifyAttribute>();
			if (filters.Any(attribute => attribute is IgnoreAttribute)) {
				//if the test should be ignored, then we bail immediately without executing any other filters
				throw new TestIgnoredException(((IgnoreAttribute)filters.First(attribute => attribute is IgnoreAttribute)).Reason);
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
		/// <exception cref="Test.TestErredException"/>
		protected virtual void OnFilterError(Exception exception, object filter, ExecutionContext executionContext) {
			var message = string.Format("Encountered an error while trying to run method filter \"{0}\"", filter.GetType().FullName);
			throw new TestErredException(exception, message);
		}

		/// <summary>
		/// Handles invocation exceptions. This is relevant because we're invoking the
		/// test method via reflection, and any exception thrown from there raises a
		/// TargetInvocationException. This method examines the InnerException (if any)
		/// and sets the result accordingly.
		/// </summary>
		/// <exception cref="Test.TestFailedException">If an assertion failed</exception>
		/// <exception cref="Test.TestErredException">If anything other a TestAssertionException was thrown</exception>
		private static void HandleException(Exception exception, ExecutionContext executionContext) {
			var actualException = exception.GetInnermostException();
			if (actualException is TestAssertionException) {
				//an assertion failed
				throw new TestFailedException(actualException.Message);
			}

			throw new TestErredException(actualException, actualException.Message);
		}

		/// <summary>
		/// Creates the test result that will be attached to the execution
		/// context after the test has been run
		/// </summary>
		protected override ITestResult CreateTestResult() {
			var result = new TestMethodResult(this);
			result.SetLogger(Logger);
			return result;
		}

		/// <summary>
		/// Invokes the test method with zero arguments, using the execution context's 
		/// Instance as the invocation context
		/// </summary>
		protected virtual void RunTestMethod(ExecutionContext executionContext) {
			method.Invoke(executionContext.Instance, new object[] { });
		}

		/// <summary>
		/// Verifies that the test method is a valid, testable method
		/// </summary>
		/// <exception cref="Test.TestErredException">If the method is not runnable</exception>
		protected void VerifyMethod(ExecutionContext executionContext) {
			if (!method.IsRunnable()) {
				throw new TestErredException(null, "The test method is invalid");
			}
		}
	}
}
