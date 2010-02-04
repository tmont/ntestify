namespace NTestify.Constraint {
	/// <summary>
	/// Constraint composite that negates another constraint
	/// </summary>
	public class NotConstraint : IConstraint {
		private readonly INegatableConstraint positiveConstraint;

		public NotConstraint(INegatableConstraint positiveConstraint) {
			this.positiveConstraint = positiveConstraint;
		}

		///<inheritdoc/>
		public bool Validate() {
			return !positiveConstraint.Validate();
		}

		///<inheritdoc/>
		public string FailMessage {
			get { return positiveConstraint.NegatedFailMessage; }
		}
	}
}