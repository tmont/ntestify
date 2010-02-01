namespace NTestify.Constraint {
	public class NotConstraint<T> : IConstraint<IConstraint<T>> {
		private readonly IConstraint<T> positiveConstraint;

		public NotConstraint(IConstraint<T> positiveConstraint) {
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