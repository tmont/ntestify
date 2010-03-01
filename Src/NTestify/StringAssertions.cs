using NTestify.Constraint;

namespace NTestify {
	public static partial class Assert {

		#region Contains
		/// <summary>
		/// Asserts that a string contains another string
		/// </summary>
		/// <param name="haystack">The string to search</param>
		/// <param name="needle">The string to look for</param>
		public static void Contains(string haystack, string needle) {
			ExecuteConstraint(new StringContainsConstraint(haystack, needle), null);
		}

		/// <summary>
		/// Asserts that a string contains another string
		/// </summary>
		/// <param name="haystack">The string to search</param>
		/// <param name="needle">The string to look for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void Contains(string haystack, string needle, string message) {
			ExecuteConstraint(new StringContainsConstraint(haystack, needle), message);
		}

		/// <summary>
		/// Asserts that a string does not contain another string
		/// </summary>
		/// <param name="haystack">The string to search</param>
		/// <param name="needle">The string to look for</param>
		public static void NotContains(string haystack, string needle) {
			Not(new StringContainsConstraint(haystack, needle), needle);
		}

		/// <summary>
		/// Asserts that a string does not contain another string
		/// </summary>
		/// <param name="haystack">The string to search</param>
		/// <param name="needle">The string to look for</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void NotContains(string haystack, string needle, string message) {
			Not(new StringContainsConstraint(haystack, needle), needle);
		}
		#endregion

		#region Matches
		/// <summary>
		/// Asserts that a string matches a regular expression
		/// </summary>
		/// <param name="haystack">The string to evaluate</param>
		/// <param name="regex">The regular expression to match</param>
		public static void Matches(string haystack, string regex) {
			ExecuteConstraint(new StringMatchesRegexConstraint(haystack, regex), null);
		}

		/// <summary>
		/// Asserts that a string matches a regular expression
		/// </summary>
		/// <param name="haystack">The string to evaluate</param>
		/// <param name="regex">The regular expression to match</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void Matches(string haystack, string regex, string message) {
			ExecuteConstraint(new StringMatchesRegexConstraint(haystack, regex), message);
		}

		/// <summary>
		/// Asserts that a string does not match a regular expression
		/// </summary>
		/// <param name="haystack">The string to evaluate</param>
		/// <param name="regex">The regular expression to match</param>
		public static void NotMatches(string haystack, string regex) {
			Not(new StringMatchesRegexConstraint(haystack, regex), null);
		}

		/// <summary>
		/// Asserts that a string does not match a regular expression
		/// </summary>
		/// <param name="haystack">The string to evaluate</param>
		/// <param name="regex">The regular expression to match</param>
		/// <param name="message">The message to display to the user if the assertion fails</param>
		public static void NotMatches(string haystack, string regex, string message) {
			Not(new StringMatchesRegexConstraint(haystack, regex), message);
		}
		#endregion

	}
}