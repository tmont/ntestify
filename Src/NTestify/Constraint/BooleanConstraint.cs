namespace NTestify.Constraint {
	/// <summary>
	/// Constraint for validating that a boolean value
	/// </summary>
	public class BooleanConstraint : SimpleConstraint<object> {
		public BooleanConstraint(bool expected, object actual) : base(expected, actual) { }

		///<inheritdoc/>
		public override bool Validate() {
			if (Actual == null) {
				ActualDisplayName = "null";
				return false;
			}

			var actualType = Actual.GetType();
			if (actualType != Expected.GetType()) {
				ActualDisplayName = "an object of type " + actualType.GetFriendlyName();
				return false;
			}

			if (!Expected.Equals(Actual)) {
				ActualDisplayName = (bool)Expected ? "false" : "true";
				return false;
			}

			return true;
		}

		///<inheritdoc/>
		protected override string ExpectedDisplayName {
			get { return (bool)Expected ? "true" : "false"; }
		}
	}

}