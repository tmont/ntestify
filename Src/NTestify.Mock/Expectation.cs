using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTestify.Mock {
	/// <summary>
	/// Expectation for property getters and methods that have a return type
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	/// <typeparam name="TReturn">The type of the return value</typeparam>
	public class Expectation<T, TReturn> : InvocationExpectation<T, Expression<Func<T, TReturn>>> where T : class {
		private readonly List<TReturn> returnValues;

		/// <summary>
		/// Invokes the expectation
		/// </summary>
		/// <exception cref="MockInvocationException">If the number of invocations exceeds the xpected</exception>
		public TReturn Invoke() {
			ActualInvocationCount++;
			if (ActualInvocationCount > ExpectedInvocationCount) {
				throw CreateInvocationException(ExpectedInvocationCount, ActualInvocationCount);
			}

			return ReturnValues[ActualInvocationCount - 1];
		}

		/// <summary>
		/// Return values for this invocation, ordered by the expected order of
		/// invocation (e.g. the return value at index 0 is expected to be returned
		/// the first invocation, at index 1 the second, and so on)
		/// </summary>
		public IList<TReturn> ReturnValues { get { return returnValues; } }

		/// <summary>
		/// Creates a new expectation with an invocation that requires a return value
		/// </summary>
		/// <param name="invocation">The expected invocation</param>
		public Expectation(Expression<Func<T, TReturn>> invocation)
			: base(invocation) {
			returnValues = new List<TReturn>();
		}

		/// <summary>
		/// Adds a range of return values to the ReturnValues enumerable
		/// </summary>
		public void AddReturnValues(IEnumerable<TReturn> values) {
			if (values.Count() != ExpectedInvocationCount) {
				throw new ArgumentException(
					string.Format(
						"The number of return values must be equal to the expected invocation count: expected {0} but got {1}",
						ExpectedInvocationCount,
						values.Count()
					),
					"values"
				);
			}

			returnValues.AddRange(values);
		}

	}

	/// <summary>
	/// Expectation for property setters and methods that don't have
	/// return values
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	public class Expectation<T> : InvocationExpectation<T, Expression<Action<T>>> where T : class {
		/// <summary>
		/// Creates a new expectation with an invocation with no return value
		/// </summary>
		/// <param name="invocation">The expected invocation</param>
		public Expectation(Expression<Action<T>> invocation) : base(invocation) { }

		/// <summary>
		/// Invokes the expectation
		/// </summary>
		/// <exception cref="MockInvocationException">If the number of invocations exceeds the xpected</exception>
		public void Invoke() {
			ActualInvocationCount++;

			if (ActualInvocationCount > ExpectedInvocationCount) {
				throw CreateInvocationException(ExpectedInvocationCount, ActualInvocationCount);
			}
		}

	}

}