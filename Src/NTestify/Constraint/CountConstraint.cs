using System;
using System.Collections.Generic;
using System.Linq;

namespace NTestify.Constraint {
	/// <summary>
	/// Constraint that validates the count of an IEnumerable
	/// </summary>
	public class CountConstraint : EnumerableConstraint<IEnumerable<object>, int> {
		public CountConstraint(IEnumerable<object> enumerable, int count) : base(enumerable, count){
			if (enumerable == null) throw new ArgumentNullException("enumerable"); //this should never happen; boy, have i heard that before
		}

		///<inheritdoc/>
		public override bool Validate() {
			return Enumerable.Count() == Value;
		}

		///<inheritdoc/>
		public override string FailMessage {
			get {
				return string.Format(
					"Failed asserting that the number of items in an object of type {0} is {1}.",
					Enumerable.GetType().GetFriendlyName(),
					Value
				);
			}
		}

		///<inheritdoc/>
		public override string NegatedFailMessage { get { return FailMessage.Replace(" is ", " is not "); } }
	}
}