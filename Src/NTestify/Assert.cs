using System.ComponentModel;
using NTestify.Constraint;

namespace NTestify {

	/// <summary>
	/// Provides access to common assertions
	/// </summary>
	public static partial class Assert {

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool ReferenceEquals(object x, object y) {
			return object.ReferenceEquals(x, y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object x, object y) {
			return object.Equals(x, y);
		}

		/// <summary>
		/// Provides access to assertion extension methods
		/// </summary>
		public static readonly AssertionExtensionPoint Ext = new AssertionExtensionPoint();

		/// <summary>
		/// Negates and validates a constraint
		/// </summary>
		/// <param name="positiveConstraint">The constraint to negate</param>
		/// <param name="message">The message to display if the constraint is invalid</param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void Not(INegatableConstraint positiveConstraint, string message) {
			ExecuteConstraint(new NotConstraint(positiveConstraint), message);
		}

		/// <summary>
		/// Executes a constraint on a variable number of arguments
		/// </summary>
		/// <param name="constraint">The constraint to execute</param>
		/// /// <param name="message">The message to display if the constraint is invalid</param>
		/// <exception cref="TestAssertionException">If the constraint fails to validate</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void ExecuteConstraint(IConstraint constraint, string message) {
			if (!constraint.Validate()) {
				message = string.IsNullOrEmpty(message) ? string.Empty : message + "\n";
				message += constraint.FailMessage;
				throw new TestAssertionException(message);
			}
		}

	}
}