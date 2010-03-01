using NTestify.Constraint;
using System.Linq;
using System.Collections.Generic;

namespace NTestify {
	public static partial class Assert {
		#region Contains
		/// <summary>
		/// Asserts that an enumerable object contains a value
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		public static void Contains<TSource>(IEnumerable<TSource> enumerable, TSource value) {
			ExecuteConstraint(new ContainsConstraint<TSource>(enumerable, value), null);
		}

		/// <summary>
		/// Asserts that an enumerable object contains a value, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void Contains<TSource>(IEnumerable<TSource> enumerable, TSource value, string message) {
			ExecuteConstraint(new ContainsConstraint<TSource>(enumerable, value), message);
		}

		/// <summary>
		/// Asserts that an enumerable object does not contain a value
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		public static void NotContains<TSource>(IEnumerable<TSource> enumerable, TSource value) {
			Not(new ContainsConstraint<TSource>(enumerable, value), null);
		}

		/// <summary>
		/// Asserts that an enumerable object does not contain a value, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="value">The value to search for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void NotContains<TSource>(IEnumerable<TSource> enumerable, TSource value, string message) {
			Not(new ContainsConstraint<TSource>(enumerable, value), message);
		}
		#endregion

		#region Count
		/// <summary>
		/// Safely casts a non-generic IEnumerable
		/// </summary>
		private static IEnumerable<object> SafeCast<T>(IEnumerable<T> enumerable) {
			return (enumerable == null) ? Enumerable.Empty<object>() : enumerable.Cast<object>();
		}

		/// <summary>
		/// Asserts that an enumerable contains a certain number of items, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="count">The number of items expected to be in the enumerable</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void Count<TSource>(IEnumerable<TSource> enumerable, int count, string message) {
			ExecuteConstraint(new CountConstraint(SafeCast(enumerable), count), message);
		}

		/// <summary>
		/// Asserts that an enumerable contains a certain number of items
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="count">The number of items expected to be in the enumerable</param>
		public static void Count<TSource>(IEnumerable<TSource> enumerable, int count) {
			ExecuteConstraint(new CountConstraint(SafeCast(enumerable), count), null);
		}

		/// <summary>
		/// Asserts that an enumerable does not contain a certain number of items, displaying the specified message upon failure
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="count">The number of items expected to be in the enumerable</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void NotCount<TSource>(IEnumerable<TSource> enumerable, int count, string message) {
			Not(new CountConstraint(SafeCast(enumerable), count), message);
		}

		/// <summary>
		/// Asserts that an enumerable does not contain a certain number of items
		/// </summary>
		/// <typeparam name="TSource">The generic enumerable type</typeparam>
		/// <param name="enumerable">The enumerable object to search</param>
		/// <param name="count">The number of items expected to be in the enumerable</param>
		public static void NotCount<TSource>(IEnumerable<TSource> enumerable, int count) {
			Not(new CountConstraint(SafeCast(enumerable), count), null);
		}
		#endregion

	}
}