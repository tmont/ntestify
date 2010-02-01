using System;
using System.Collections;
using System.Linq;

namespace NTestify.Constraint {
	public class EqualConstraint : BinaryConstraint<object> {

		public EqualConstraint(object expected, object actual) : base(expected, actual) { }

		private string reasonForFailure;

		public override bool Validate() {
			if (ReferenceEquals(Expected, Actual)) {
				//short circuit if they are the same reference
				return true;
			}

			var expectedType = Expected.GetType();
			var actualType = Actual.GetType();

			if (expectedType.IsValueType && actualType.IsValueType) {
				//if they are both value types, a simple comparison is all that is needed
				if (Expected != Actual) {
					reasonForFailure = string.Format("{0} is not equal to {1}", Expected, Actual);
					return false;
				}

				return true;
			}

			if (!Expected.GetType().Equals(Actual.GetType())) {
				//if they're not the same type, that's a big fail
				reasonForFailure = "They are not the same underlying type";
				return false;
			}

			if (Expected is IList) {
				//lists can be handled separately
				return CompareLists((IList)Expected, (IList)Actual);
			}

			//if all else fails, fall back to Equals()
			if (!Expected.Equals(Actual)) {
				reasonForFailure = "Expected.Equals(Actual) returned false";
				return false;
			}

			return true;
		}

		protected bool CompareLists(IList expected, IList actual) {
			if (expected.Count != actual.Count) {
				//different number of items
				var difference = Math.Abs(expected.Count - actual.Count);
				reasonForFailure = string.Format(
					"Expected has {0} {1} {2} than Actual",
					difference,
					(expected.Count < actual.Count) ? "less" : "more",
					(difference == 1) ? "item" : "items"
					);

				return false;
			}

			foreach (var item in expected.Cast<object>().Where(item => !actual.Contains(item))) {
				//actual does not contain an item in expected
				reasonForFailure = string.Format(
					"Actual does not contain an item in Expected: {0}",
					item
					);
				return false;
			}

			return true;
		}

		public override string FailMessage {
			get { 
				return string.Format(
					"Failed asserting that two objects are equal: expected {0}, but got {1}.\n{2}", 
					Expected.GetType().FullName,
					Actual.GetType().FullName,
					reasonForFailure
					); 
			}
		}
	}
}