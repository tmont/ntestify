using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Represents a single testable method, i.e. an instance method that is annotated
	/// with the [Test] attribute
	/// </summary>
	public class ReflectedTestMethod : Test {
		private readonly MethodInfo method;
		private readonly object instance;

		/// <param name="method">The test method to run. Cannot be null.</param>
		/// <param name="instance">The object instance that the method belongs to</param>
		public ReflectedTestMethod(MethodInfo method, object instance) {
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			if (instance == null) {
				throw new ArgumentNullException("instance");
			}

			this.method = method;
			this.instance = instance;
			Name = instance.GetType().Name + "." + method.Name;
		}

		/// <summary>
		/// Sets the object instance on the execution context
		/// </summary>
		protected override void InitializeContext(ExecutionContext executionContext) {
			executionContext.Instance = instance;
		}

		/// <inheritdoc/>
		protected override void RunTest(ExecutionContext executionContext) {
			VerifyMethod();

			try {
				RunTestMethod(executionContext);
			} catch (Exception exception) {
				HandleException(exception);
			}
		}

		/// <summary>
		/// Finds and executes each of the pre test filters in order, including any Setup filters
		/// attached to the class, if there are any
		/// </summary>
		/// <exception cref="Test.TestIgnoredException">If the method is annotated with [Ignore]</exception>
		protected override void RunPreTestFilters(ExecutionContext executionContext) {
			var filters = method.GetAttributes<PreTestFilter>();
			if (filters.Any(attribute => attribute is IgnoreAttribute)) {
				//if the test should be ignored, then we bail immediately without executing any other filters
				throw new TestIgnoredException(filters.Where(attribute => attribute is IgnoreAttribute).Cast<IgnoreAttribute>().First().Reason);
			}

			filters = filters.Union(GetSetupMethods(method.DeclaringType));
			RunFilters(filters, executionContext);
		}

		private static IEnumerable<PreTestFilter> GetSetupMethods(Type declaringClass) {
			return declaringClass
				.GetMethods()
				.Where(m => m.HasAttribute<SetupAttribute>())
				.Select(m => {
					var setup = m.GetAttributes<SetupAttribute>().First();
					setup.Method = m;
					return setup;
				})
				.Cast<PreTestFilter>()
				.OrderBy(filter => filter.Order);
		}

		/// <summary>
		/// Finds and executes each of the method filters
		/// </summary>
		protected override void RunPostTestFilters(ExecutionContext executionContext) {
			var filters = method
				.GetAttributes<PostTestFilter>()
				.Union(GetTearDownMethods(method.DeclaringType));
			RunFilters(filters, executionContext);
		}

		private static IEnumerable<PostTestFilter> GetTearDownMethods(Type declaringClass) {
			return declaringClass
				.GetMethods()
				.Where(m => m.HasAttribute<TearDownAttribute>())
				.Select(m => {
					var tearDown = m.GetAttributes<TearDownAttribute>().First();
					tearDown.Method = m;
					return tearDown;
				})
				.Cast<PostTestFilter>()
				.OrderBy(filter => filter.Order);
		}

		private void RunFilters<TFilter>(IEnumerable<TFilter> filters, ExecutionContext executionContext) where TFilter : TestFilter {
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
		/// <exception cref="Test.TestErredException"/>
		protected virtual void OnFilterError(Exception exception, object filter, ExecutionContext executionContext) {
			var message = string.Format("Encountered an error while trying to run method filter of type \"{0}\"", filter.GetType().GetFriendlyName());
			throw new TestErredException(exception, message);
		}

		/// <summary>
		/// Handles exceptions raised during a test
		/// </summary>
		/// <exception cref="Test.TestFailedException">If an assertion failed</exception>
		/// <exception cref="Test.TestErredException">If anything other a TestAssertionException was thrown</exception>
		private static void HandleException(Exception exception) {
			var actualException = exception.GetBaseException();
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
			return new TestMethodResult(this) {
				Logger = Logger
			};
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
		private void VerifyMethod() {
			if (!method.IsRunnable()) {
				throw new TestErredException(null, "The test method is invalid");
			}
		}
	}
}
