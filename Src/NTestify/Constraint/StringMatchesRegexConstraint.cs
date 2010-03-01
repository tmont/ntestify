using System.Text.RegularExpressions;

namespace NTestify.Constraint {

	/// <summary>
	/// Constraint to determine if a string matches a regular expression
	/// </summary>
	public class StringMatchesRegexConstraint : BinaryConstraint<string> {
		public StringMatchesRegexConstraint(string haystack, string regex) : base(haystack, regex) { }

		public override bool Validate() {
			return string.IsNullOrEmpty(Expected) ? string.IsNullOrEmpty(Actual) : Regex.IsMatch(Expected, Actual ?? string.Empty);
		}

		public override string FailMessage {
			get { return string.Format("Failed asserting that the string \"{0}\" matches the regular expression \"{1}\".", Expected, Actual); }
		}

		public override string NegatedFailMessage {
			get { return FailMessage.Replace(" matches ", " does not match "); }
		}
	}
}