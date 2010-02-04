using NTestify.Constraint;

namespace NTestify {

	public static partial class Assert {
		/// <summary>
		/// Asserts that two objects are equal
		/// </summary>
		/// <param name="expected">The object that is expected to be returned</param>
		/// <param name="actual">The object that was actually returned</param>
		public static void Equal(object expected, object actual) {
			ExecuteConstraint<EqualConstraint, object>(null, expected, actual);
		}

		/// <summary>
		/// Asserts that two objects are equal, displaying the specified message upon failure
		/// </summary>
		/// <param name="expected">The object that is expected to be returned</param>
		/// <param name="actual">The object that was actually returned</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void Equal(object expected, object actual, string message) {
			ExecuteConstraint<EqualConstraint, object>(message, expected, actual);
		}

		/// <summary>
		/// Asserts that two objects are not equal
		/// </summary>
		/// <param name="expected">The object that is expected to be returned</param>
		/// <param name="actual">The object that was actually returned</param>
		public static void NotEqual(object expected, object actual) {
			Not<EqualConstraint, object>(null, expected, actual);
		}

		/// <summary>
		/// Asserts that two objects are not equal
		/// </summary>
		/// <param name="expected">The object expected to be returned</param>
		/// <param name="actual">The object that was actually returned</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void NotEqual(object expected, object actual, string message) {
			Not<EqualConstraint, object>(message, expected, actual);
		}
	}

}