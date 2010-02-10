using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTestify.Mock {

	internal interface IExpectation {
		int ExpectedInvocationCount { get; set; }
		int ActualInvocationCount { get; }
	}

	/// <summary>
	/// Represents an expectation. For example, "a method is expected
	/// to be invoked once and return foo."
	/// </summary>
	/// <typeparam name="T">The type this invocation expects to operate on</typeparam>
	/// <typeparam name="TInvocation">The type of delegate the type invokes</typeparam>
	internal abstract class InvocationExpectation<T, TInvocation> : IExpectation where T : class where TInvocation : Expression {
		private int expectedInvocationCount;
		private readonly TInvocation invocation;

		protected InvocationExpectation(TInvocation invocation) {
			ExpectedInvocationCount = 1;
			this.invocation = invocation;
		}

		/// <summary>
		/// Gets or sets the number of times this invocation is expected to be invoked. The default value
		/// is one, so if this expectation is never expected to be invoked, explicitly set
		/// this value to zero.
		/// </summary>
		/// <value>The number of times this invocation is expected to be invoked; must be greater than or equal to zero</value>
		public int ExpectedInvocationCount {
			get { return expectedInvocationCount; }
			set {
				if (value < 0) {
					throw new ArgumentException("Invocation count must be greater than or equal to zero", "value");
				}

				expectedInvocationCount = value;
			}
		}

		/// <summary>
		/// Gets the number of times this expectation has been invoked
		/// </summary>
		public int ActualInvocationCount { get; protected set; }

		/// <summary>
		/// The callback that is expected to be executed when the expectation
		/// is invoked
		/// </summary>
		public Action<T> Callback { get; set; }

		/// <summary>
		/// The invocation for this expectation
		/// </summary>
		public TInvocation Invocation { get { return invocation; } }

		/// <summary>
		/// Creates and formats an exception to be raised when the number of actual
		/// invocations exceeds the expected
		/// </summary>
		protected MockInvocationException CreateInvocationException(int expected, int actual) {
			return new MockInvocationException(
			   string.Format(
				   "Invocation {0} was invoked {1} {2}, but only expected to be invoked {3} {4}",
				   ToString(),
				   expected,
				   (expected == 1) ? "time" : "times",
				   actual,
				   (actual == 1) ? "time" : "times"
			   )
		   );
		}

		public override string ToString() {
			return Invocation.ToString();
		}
	}

	/// <summary>
	/// Expectation for property getters and methods that have a return type
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	/// <typeparam name="TReturn">The type of the return value</typeparam>
	internal class Expectation<T, TReturn> : InvocationExpectation<T, Expression<Func<T, TReturn>>> where T : class {
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

	public class MockInvocationException : Exception {
		public MockInvocationException(string message) : base(message) { }
	}

	/// <summary>
	/// Expectation for property setters and methods that don't have
	/// return values
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	internal class Expectation<T> : InvocationExpectation<T, Expression<Action<T>>> where T : class {
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