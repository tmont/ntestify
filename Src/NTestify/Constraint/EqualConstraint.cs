using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTestify.Constraint {

	public class EqualConstraint : BinaryConstraint<object> {

		public EqualConstraint(object expected, object actual) : base(expected, actual) { }

		private string reasonForFailure;

		public override bool Validate() {
			return AreEqual(Expected, Actual);
		}

		private bool AreEqual(object expected, object actual) {
			if (ReferenceEquals(expected, actual)) {
				//short circuit if they are the same reference
				return true;
			}

			if (actual == null || expected == null) {
				if (expected == null) {
					if (actual != null) {
						reasonForFailure = string.Format("Expected null, got object of type {0}.", actual.GetType().UnderlyingSystemType.GetFriendlyName());
						return false;
					}

					return true;
				}

				reasonForFailure = string.Format("Expected object of type {0}, got null.", expected.GetType().UnderlyingSystemType.GetFriendlyName());
				return false;
			}

			//at this point, neither are null, so calling GetType() is safe from null references
			var expectedType = expected.GetType();
			var actualType = actual.GetType();

			//numbers are special, because we want 1 == 1.0 == 1.0m == 1.0f
			if (expectedType.IsNumeric() && actualType.IsNumeric()) {
				if (!expected.Equals(Convert.ChangeType(actual, expectedType))) {
					reasonForFailure = string.Format("{1} is not equal to {0}.", expected, actual);
					return false;
				}

				return true;
			}

			if (!expectedType.Equals(actualType)) {
				//if they're not the same type, that's a big fail
				reasonForFailure = string.Format(
					"{1} is not a {0}.",
					expectedType.UnderlyingSystemType.GetFriendlyName(),
					actualType.UnderlyingSystemType.GetFriendlyName()
				);
				return false;
			}

			if (expected is string || expected is char || expected is bool) {
				//prettify the failure messages for primitive types
				if (!expected.Equals(actual)) {
					if (expected is string) {
						reasonForFailure = string.Format("Expected \"{0}\"\nbut got  \"{1}\"", expected, actual);
					} else if (expected is char) {
						reasonForFailure = string.Format("'{1}' is not equal to '{0}'.", expected, actual);
					} else {
						//bool
						reasonForFailure = string.Format("{1} is not equal to {0}.", expected, actual);
					}

					return false;
				}

				return true;
			}

			if (expected is ICollection) {
				//collection will be handled separately
				return CompareCollections((ICollection)expected, (ICollection)actual);
			}

			//if we get here, then both types are complex non-collections, so all we can do is fall back to Equals()
			if (!expected.Equals(actual)) {
				reasonForFailure = "object.Equals() returned false.";
				return false;
			}

			return true;
		}

		private bool CompareCollections(ICollection expected, ICollection actual) {
			if (expected.Count != actual.Count) {
				//different number of items
				var difference = Math.Abs(expected.Count - actual.Count);
				reasonForFailure = string.Format(
					"Expected has {0} {1} {2} than Actual.",
					difference,
					(expected.Count < actual.Count) ? "less" : "more",
					(difference == 1) ? "item" : "items"
				);

				return false;
			}

			if (expected is IDictionary) {
				return CompareDictionaries((IDictionary)expected, (IDictionary)actual);
			}

			//if it's not a dictionary, it's a list
			return CompareLists((IList)expected, (IList)actual);
		}

		private bool CompareLists(IList expected, IList actual) {
			for (var i = 0; i < expected.Count; i++) {
				if (!AreEqual(expected[i], actual[i])) {
					//actual does not contain an item in expected
					reasonForFailure = string.Format(
						"Actual does not contain an item in Expected at index {0}: {1}.",
						i,
						expected[i]
					);
					return false;
				}

			}

			return true;
		}

		private bool CompareDictionaries(IDictionary expected, IDictionary actual) {
			foreach (var key in expected.Keys) {
				if (!actual.Contains(key)) {
					reasonForFailure = string.Format(
						"Actual does not contain key {0}.",
						(key is string || key.GetType().IsPrimitive) ? key : "fix me" //TODO this needs to show some sort of friendly object description
					);
					return false;
				}
				if (!AreEqual(expected[key], actual[key])) {
					reasonForFailure = string.Format(
						//TODO this failure message should use the failure reason that AreEqual() returns
						"Actual value does not match expected value at key {0}: {1}.",
						key,
						expected[key]
					);
					return false;
				}
			}

			return true;
		}

		public override string FailMessage {
			get {
				return string.Format(
					"Failed asserting that two objects are equal.\n{0}",
					reasonForFailure
				);
			}
		}
	}
}