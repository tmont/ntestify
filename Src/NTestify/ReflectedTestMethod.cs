﻿using System;
using System.Linq;
using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Represents a single testable method, i.e. an instance method that is annotated
	/// with the [Test] attribute
	/// </summary>
	public class ReflectedTestMethod : Test, ITestMethodInfo {
		private readonly object instance;

		/// <summary>
		/// Gets the method to invoke to run this test
		/// </summary>
		public MethodInfo Method { get; protected set; }

		/// <param name="method">The test method to run. Cannot be null.</param>
		/// <param name="instance">The object instance that the method belongs to, or null if a static method</param>
		public ReflectedTestMethod(MethodInfo method, object instance) {
			if (method == null) {
				throw new ArgumentNullException("method");
			}

			Method = method;
			this.instance = instance;
			Name = Method.DeclaringType.Name + "." + method.Name;

			ApplyAttributeDetails();
			ApplyExpectedExceptionDetails();
		}

		private void ApplyAttributeDetails() {
			var testInfo = Method
				.GetAttributes<TestifyAttribute>()
				.Where(a => a is ITestInfo)
				.Cast<ITestInfo>()
				.FirstOrDefault();

			if (testInfo == null) {
				return;
			}

			Name = testInfo.Name ?? Name;
			Description = testInfo.Description;
			Category = testInfo.Category;
		}

		private void ApplyExpectedExceptionDetails() {
			var exceptionDetails = Method
				.GetAttributes<ExpectedExceptionAttribute>()
				.FirstOrDefault();

			if (exceptionDetails == null) {
				return;
			}

			ExpectedException = exceptionDetails.ExpectedException;
			ExpectedExceptionMessage = exceptionDetails.ExpectedMessage;
		}

		/// <summary>
		/// Sets the object instance on the execution context
		/// </summary>
		protected override void InitializeContext(ExecutionContext executionContext) {
			executionContext.Instance = instance;
		}

		/// <summary>
		/// Runs the test. This method should do the actual heavy lifting.
		/// It should use the internal exceptions to signify the result of
		/// the test, rather than setting the result property itself.
		/// </summary>
		protected override void RunTest(ExecutionContext executionContext) {
			if (!Method.IsRunnable()) {
				throw new TestErredException(null, "The test method is invalid");
			}

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
			var filters = Method.GetFilters<PreTestFilter>();
			if (filters.Any(attribute => attribute is IgnoreAttribute)) {
				//if the test should be ignored, then we bail immediately without executing any other filters
				throw new TestIgnoredException(filters.Where(filter => filter is IgnoreAttribute).Cast<IgnoreAttribute>().First().Reason);
			}

			filters = filters.Concat(Method.DeclaringType.GetInvokableFilters<SetupAttribute>());
			RunFiltersInOrder(filters, executionContext);
		}

		/// <summary>
		/// Finds and executes each of the method filters
		/// </summary>
		protected override void RunPostTestFilters(ExecutionContext executionContext) {
			var filters = Method
				.GetFilters<PostTestFilter>()
				.Concat(Method.DeclaringType.GetInvokableFilters<TearDownAttribute>());

			RunFiltersInOrder(filters, executionContext);
		}

		/// <summary>
		/// Handles exceptions raised during a test
		/// </summary>
		/// <exception cref="Test.TestFailedException">If an assertion failed</exception>
		/// <exception cref="Test.TestErredException">If anything other than a TestAssertionException was thrown</exception>
		private static void HandleException(Exception exception) {
			var actualException = exception.GetBaseException();
			if (actualException is TestAssertionException) {
				//an assertion failed
				throw new TestFailedException(actualException.Message, actualException);
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
		/// Instance property as the invocation context
		/// </summary>
		protected virtual void RunTestMethod(ExecutionContext executionContext) {
			Method.Invoke(executionContext.Instance, new object[0]);
		}
	}
}
