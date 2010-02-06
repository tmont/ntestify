using System;
using System.Collections.Generic;
using System.Linq;

namespace NTestify.Mock {

	/// <summary>
	/// Represents an expectation. For example, "a method is expected
	/// to be invoked once and return foo."
	/// </summary>
	/// <typeparam name="T">The type this invocation expects to operate on</typeparam>
	internal abstract class InvocationExpectation<T> where T : class {
		private int expectedInvocationCount;

		protected InvocationExpectation() {
			ExpectedInvocationCount = 1;
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
		/// The callback that is expected to be executed when the expectation
		/// is invoked
		/// </summary>
		public Action<T> Callback { get; set; }
	}

	/// <summary>
	/// Expectation for property getters and methods that have a return type
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	/// <typeparam name="TReturn">The type of the return value</typeparam>
	internal class Expectation<T, TReturn> : InvocationExpectation<T> where T : class {
		private readonly List<TReturn> returnValues;
		private readonly Func<T, TReturn> invocation;

		/// <summary>
		/// The invocation for this expectation
		/// </summary>
		public Func<T, TReturn> Invocation { get { return invocation; } }

		/// <summary>
		/// Return values for this invocation, ordered by the expected order of
		/// invocation (e.g. the return value at index 0 is expected to be returned
		/// the first invocation, at index 1 the second, and so on)
		/// </summary>
		public IEnumerable<TReturn> ReturnValues { get { return returnValues; } }

		/// <summary>
		/// Creates a new expectation with an invocation that requires a return value
		/// </summary>
		/// <param name="invocation">The expected invocation</param>
		public Expectation(Func<T, TReturn> invocation) {
			returnValues = new List<TReturn>();
			this.invocation = invocation;
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
	internal class Expectation<T> : InvocationExpectation<T> where T : class {
		private readonly Action<T> invocation;

		/// <summary>
		/// Creates a new expectation with an invocation with no return value
		/// </summary>
		/// <param name="invocation">The expected invocation</param>
		public Expectation(Action<T> invocation) {
			this.invocation = invocation;
		}

		/// <summary>
		/// The invocation for this expectation
		/// </summary>
		public Action<T> Invocation {
			get { return invocation; }
		}
	}

}