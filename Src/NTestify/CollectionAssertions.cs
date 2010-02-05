using NTestify.Constraint;
using System.Collections.Generic;

namespace NTestify {
	public static partial class Assert {
		/// <summary>
		/// Asserts that an enumerable object contains a value
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type; use object if generics are irrelevant</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		public static void Contains<TSource>(IEnumerable<TSource> enumerable, TSource value) {
			ExecuteConstraint(new ContainsConstraint<TSource>(enumerable, value), null);
		}

		/// <summary>
		/// Asserts that an enumerable object contains a value, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type; use object if generics are irrelevant</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void Contains<TSource>(IEnumerable<TSource> enumerable, TSource value, string message) {
			ExecuteConstraint(new ContainsConstraint<TSource>(enumerable, value), message);
		}

		/// <summary>
		/// Asserts that an enumerable object does not contain a value
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type; use object if generics are irrelevant</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		public static void NotContains<TSource>(IEnumerable<TSource> enumerable, TSource value) {
			Not(new ContainsConstraint<TSource>(enumerable, value), null);
		}

		/// <summary>
		/// Asserts that an enumerable object does not contain a value, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type; use object if generics are irrelevant</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void NotContains<TSource>(IEnumerable<TSource> enumerable, TSource value, string message) {
			Not(new ContainsConstraint<TSource>(enumerable, value), message);
		}

	}
}