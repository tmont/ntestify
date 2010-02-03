namespace NTestify.Constraint {

	/// <summary>
	/// Represents a validatable constraint on a value or values.
	/// Constraints make up the core of how assertions work in NTestify.
	/// </summary>
	public interface IConstraint {
		/// <summary>
		/// Validates the constraint, typically against an expected
		/// value and an actual value
		/// </summary>
		bool Validate();

		/// <summary>
		/// Retrieves the message to display to end-users if the constraint
		/// is invalid
		/// </summary>
		string FailMessage { get; }
	}

}