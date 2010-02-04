namespace NTestify.Constraint {
	/// <summary>
	/// Common base class for constraints that don't need to do anything
	/// complicated, like asserting that a value is true
	/// </summary>
	/// <typeparam name="T">The type of objects to compare</typeparam>
	public abstract class SimpleConstraint<T> : BinaryConstraint<T> {
		protected SimpleConstraint(T expected, T actual) : base(expected, actual) {}

		/// <summary>
		/// The display name of the actual value
		/// </summary>
		protected string ActualDisplayName { get; set; }

		/// <summary>
		/// The display name of the expected value
		/// </summary>
		protected abstract string ExpectedDisplayName { get; }

		///<inheritdoc/>
		public override string FailMessage {
			get { return string.Format("Failed asserting that {0} is {1}.", ActualDisplayName, ExpectedDisplayName); }
		}

		///<inheritdoc/>
		public override string NegatedFailMessage {
			get { return string.Format("Failed assert that {0} is not {1}.", ActualDisplayName ?? ExpectedDisplayName, ExpectedDisplayName); }
		}
	}
}