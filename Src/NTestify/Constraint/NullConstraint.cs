namespace NTestify.Constraint {
	/// <summary>
	/// Constraint for validating null values
	/// </summary>
	public class NullConstraint : SimpleConstraint<object> {
		public NullConstraint(object actual) : base(null, actual) { }

		///<inheritdoc/>
		public override bool Validate() {
			if (Actual == null) {
				return true;
			}

			ActualDisplayName = "an object of type " + Actual.GetType().GetFriendlyName();
			return false;
		}

		///<inheritdoc/>
		protected override string ExpectedDisplayName {
			get { return "null"; }
		}
	}
}