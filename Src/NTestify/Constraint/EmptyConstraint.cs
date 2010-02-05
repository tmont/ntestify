using System.Collections;
using System.Text;
using System.Linq;

namespace NTestify.Constraint {
	/// <summary>
	/// Constraint that validates whether an expected value is "empty". The following values
	/// are considered empty: null, the empty string, a StringBuilder with Length = 0, and
	/// an IEnumerable implementation with no items. Everything else is considered non-empty.
	/// </summary>
	public class EmptyConstraint : INegatableConstraint {
		private readonly object actual;
		private string actualDisplayName;

		public EmptyConstraint(object actual) {
			this.actual = actual;
		}

		///<inheritdoc/>
		public bool Validate() {
			if (actual == null) {
				return true;
			}

			if (actual is string || actual is StringBuilder) {
				if (!string.IsNullOrEmpty(actual.ToString())) {
					actualDisplayName = "the string \"" + actual + "\"";
					return false;
				}

				return true;
			}

			if (actual is IEnumerable) {
				var count = ((IEnumerable)actual).Cast<object>().Count();
				if (count > 0) {
					actualDisplayName = string.Format(
						"an IEnumerable of type {0} (count={1})",
						actual.GetType().GetFriendlyName(),
						count
					);
					return false;
				}

				return true;
			}

			actualDisplayName = "an object of type " + actual.GetType().GetFriendlyName();
			return false;
		}

		///<inheritdoc/>
		public string FailMessage {
			get { return string.Format("Failed asserting that {0} is empty.", actualDisplayName); }
		}

		///<inheritdoc/>
		public string NegatedFailMessage {
			get { return FailMessage.Replace(" is ", " is not "); }
		}
	}
}