namespace NTestify.Constraint {
	public class NotConstraint : IConstraint {
		private readonly IConstraint positiveConstraint;

		public NotConstraint(IConstraint positiveConstraint) {
			this.positiveConstraint = positiveConstraint;
		}

		public bool Validate() {
			return !positiveConstraint.Validate();
		}

		public string FailMessage {
			get { return string.Format("The opposite of the following: \"{0}\"", positiveConstraint.FailMessage); }
		}
	}
}