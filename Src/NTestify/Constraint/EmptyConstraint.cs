using System.Collections;
using System.Text;

namespace NTestify.Constraint {
	public class EmptyConstraint : IConstraint {
		private readonly object actual;
		private string actualDisplayName;

		public EmptyConstraint(object actual){
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
				var enumerator = ((IEnumerable)actual).GetEnumerator();
				enumerator.Reset();
				if (enumerator.MoveNext()) {
					var count = 1;
					while (enumerator.MoveNext()) {
						count++;
					}

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

		public string FailMessage{
			get { return string.Format("Failed asserting that {0} is empty.", actualDisplayName); }
		}

		public string NegatedFailMessage{
			get { return string.Format("Failed asserting that {0} is not empty.", actualDisplayName); }
		}
	}
}