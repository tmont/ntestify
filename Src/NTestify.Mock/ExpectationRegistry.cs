using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTestify.Mock {
	/// <summary>
	/// Registry of all expectations. This class is used to invoke a matched
	/// expectation by dynamically created mock objects.
	/// </summary>
	public static class ExpectationRegistry {

		/// <summary>
		/// Lookup of all registered expectations. Key is the thread ID.
		/// </summary>
		private static readonly IList<IExpectation> Expectations = new List<IExpectation>();

		/// <summary>
		/// Adds an expectation to the registry
		/// </summary>
		public static void Add(IExpectation expectation) {
			Expectations.Add(expectation);
		}

		/// <summary>
		/// Gets all expectations that were registered in the current thread
		/// </summary>
		public static IEnumerable<IExpectation> GetAll() {
			return Expectations;
		}

		/// <summary>
		/// Resets the registry for the current thread. This should be executed
		/// after each test runs.
		/// </summary>
		public static void Reset() {
			Expectations.Clear();
		}

		/// <summary>
		/// Invokes an expectation without a return type that matches the given expression
		/// </summary>
		/// <typeparam name="T">The mocked type</typeparam>
		/// <param name="expectation">A property setter or void method invocation</param>
		public static void Invoke<T>(Expression<Action<T>> expectation) where T : class {
			var match = Expectations
				.Cast<Expectation<Action<T>>>()
				.FirstOrDefault(e => e.Matches(expectation));

			if (match != null) {
				match.Invoke();
			}
		}

		/// <summary>
		/// Invokes an expectation that returns a value that matches the given expression
		/// </summary>
		/// <typeparam name="T">The mocked type</typeparam>
		/// <typeparam name="TReturn">The return value's type</typeparam>
		/// <param name="expectation">A property getter or returnable method invocation</param>
		/// <returns>The return value that comes as a result of invoking the expectation</returns>
		public static TReturn Invoke<T, TReturn>(Expression<Func<T, TReturn>> expectation) where T : class {
			var match = Expectations
				.Cast<Expectation<Func<T, TReturn>, TReturn>>()
				.FirstOrDefault(e => e.Matches(expectation));

			return (match != null) ? match.Invoke() : default(TReturn);
		}

	}
}