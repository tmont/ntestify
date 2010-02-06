using System;
using System.Collections.Generic;
using NTestify.Mock.Expression;

namespace NTestify.Mock {
	
	/// <summary>
	/// A mock of a class or interface
	/// </summary>
	/// <typeparam name="T">The type to mock</typeparam>
	public class Mock<T> where T : class {

		private readonly IList<InvocationExpectation<T>> expectations;

		/// <summary>
		/// Initializes a new mock object
		/// </summary>
		public Mock() {
			expectations = new List<InvocationExpectation<T>>();
		}

		/// <summary>
		/// Creates an expectation for an invocation that has a return value
		/// </summary>
		/// <typeparam name="TReturn">The invocation's return value type</typeparam>
		/// <param name="invocation">The expected invocation. This can be a property getter or a method invocation that has a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T, TReturn> Expects<TReturn>(Func<T, TReturn> invocation) {
			var expectation = new Expectation<T, TReturn>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T, TReturn>(expectation);
		}

		/// <summary>
		/// Creates an expectation for an invocation without a return value
		/// </summary>
		/// <param name="invocation">The expected invocation. This can be a property setter or a method invocation that does not have a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T> Expects(Action<T> invocation) {
			var expectation = new Expectation<T>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T>(expectation);
		}

		/// <summary>
		/// Verifies that the expectation was met
		/// </summary>
		public void Verify() {
			
		}

		/// <summary>
		/// Gets the actual object instance for this mock
		/// </summary>
		public T Object {
			get {
				if (expectations.Count == 0) {
					return null;
				}

				return null;
			}
		}

	}

}