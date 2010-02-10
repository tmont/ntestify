using System;

namespace NTestify.Mock.Expressions {
	/// <summary>
	/// Represents the expects node in the mock expression for expectations
	/// that do not have a return value
	/// </summary>
	/// <typeparam name="T">The type of the mock object</typeparam>
	public class ExpectExpression<T> where T : class {
		private readonly Expectation<T> expectation;

		internal ExpectExpression(Expectation<T> expectation) {
			this.expectation = expectation;
		}

		/// <summary>
		/// Instructs the expectation that it will execute the specified
		/// callback upon invocation
		/// </summary>
		/// <param name="callback">The action to execute</param>
		public void WillExecuteCallback(Action<T> callback) {
			expectation.Callback = callback;
		}

		/// <summary>
		/// Instructs the expectation that it will be invoked a certain
		/// number of times. The default expected invocation count is one, so if
		/// the expectation is only expected to be invoked once, this value
		/// need not be set.
		/// </summary>
		/// <param name="invocationCount">The number of times the expectation is expected to be invoked</param>
		public void ToBeInvoked(int invocationCount) {
			expectation.ExpectedInvocationCount = invocationCount;
		}

	}

	/// <summary>
	/// Represents the expects node in the mock expression for expectations
	/// that have a return value
	/// </summary>
	/// <typeparam name="T">The type of the mock object</typeparam>
	/// <typeparam name="TReturn">The expectation's return value's type</typeparam>
	public class ExpectExpression<T, TReturn> where T : class {
		private readonly Expectation<T, TReturn> expectation;

		internal ExpectExpression(Expectation<T, TReturn> expectation) {
			this.expectation = expectation;
		}

		/// <summary>
		/// Instructs the expectation that it will execute the specified action
		/// upon invocation
		/// </summary>
		/// <param name="callback">The action to execute</param>
		public CallbackExpression<T, TReturn> WillExecuteCallback(Action<T> callback) {
			expectation.Callback = callback;
			return new CallbackExpression<T, TReturn>(expectation);
		}

		/// <summary>
		/// Instructs the expectation that it will be invoked a certain
		/// number of times. The default expected invocation count is one, so if
		/// the expectation is only expected to be invoked once, this value
		/// need not be set.
		/// </summary>
		/// <param name="invocationCount">The number of times the expectation is expected to be invoked</param>
		public InvocationCountExpression<T, TReturn> ToBeInvoked(int invocationCount) {
			expectation.ExpectedInvocationCount = invocationCount;
			return new InvocationCountExpression<T, TReturn>(expectation);
		}

		/// <summary>
		/// Instructs the expectation that it is expected to return these values
		/// upon invocation
		/// </summary>
		public void WillReturn(params TReturn[] returnValues) {
			expectation.AddReturnValues(returnValues);
		}
	}
}