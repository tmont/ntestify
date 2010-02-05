using System.Linq;
using System.Collections.Generic;

namespace NTestify.Constraint {
	/// <summary>
	/// Constraint to determine if an enumerable object contains a value
	/// </summary>
	/// <typeparam name="TSource">The generic type of the enumerable</typeparam>
	public class ContainsConstraint<TSource> : EnumerableConstraint<IEnumerable<TSource>, TSource> {
		public ContainsConstraint(IEnumerable<TSource> enumerable, TSource value) : base(enumerable, value) { }

		public override bool Validate() {
			return Enumerable != null && Enumerable.Contains(Value);
		}

		/// <inheritdoc/>
		public override string FailMessage {
			get {
				return string.Format(
					"Failed asserting that {0} contains {1}.",
					(Enumerable == null) ? "null" : "an IEnumerable of type " + Enumerable.GetType().GetFriendlyName(),
					(Value == null) ? "null" : (Value.GetType().IsValueType ? Value.ToString() : "an object of type " + Value.GetType().GetFriendlyName())
				);
			}
		}

		/// <inheritdoc/>
		public override string NegatedFailMessage { get { return FailMessage.Replace(" contains ", " does not contain "); } }
	}
}