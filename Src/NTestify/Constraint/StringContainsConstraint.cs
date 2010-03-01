namespace NTestify.Constraint {

	/// <summary>
	/// Constraint to determine if a string contains another string
	/// </summary>
	public class StringContainsConstraint : BinaryConstraint<string> {

		public StringContainsConstraint(string haystack, string needle) : base(haystack, needle) { }

		public override bool Validate(){
			return (Expected == null) ? string.IsNullOrEmpty(Actual) : Expected.Contains(Actual ?? string.Empty);
		}

		public sealed override string FailMessage {
			get { return string.Format("Failed asserting that the string \"{0}\" contains the string \"{1}\".", Expected, Actual); }
		}

		public override string NegatedFailMessage {
			get { return FailMessage.Replace(" contains ", " does not contain "); }
		}
	}
}