namespace NTestify.Constraint {
	/// <summary>
	/// Represents a constraint that compares two values
	/// </summary>
	/// <typeparam name="T">The type of object to validate</typeparam>
	public abstract class BinaryConstraint<T> : INegatableConstraint {
		protected BinaryConstraint(T expected, T actual) {
			Expected = expected;
			Actual = actual;
		}

		/// <summary>
		/// The expected value
		/// </summary>
		public T Expected { get; private set; }
		/// <summary>
		/// The actual value
		/// </summary>
		public T Actual { get; private set; }

		///<inheritdoc/>
		public abstract bool Validate();

		///<inheritdoc/>
		public abstract string FailMessage { get; }

		///<inheritdoc/>
		public abstract string NegatedFailMessage { get; }
	}
}