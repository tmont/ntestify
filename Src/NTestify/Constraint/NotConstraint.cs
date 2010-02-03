namespace NTestify.Constraint {
	/// <summary>
	/// Constraint composite that negates another constraint
	/// </summary>
	public class NotConstraint : IConstraint {
		private readonly IConstraint positiveConstraint;

		public NotConstraint(IConstraint positiveConstraint) {
			this.positiveConstraint = positiveConstraint;
		}

		///<inheritdoc/>
		public bool Validate() {
			return !positiveConstraint.Validate();
		}

		///<inheritdoc/>
		public string FailMessage {
			get { return string.Format("The opposite of the following: \"{0}\"", positiveConstraint.FailMessage); }
		}
	}
}