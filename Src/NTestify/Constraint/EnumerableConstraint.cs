using System.Collections;

namespace NTestify.Constraint {
	/// <summary>
	/// Common base class for constraints on collections
	/// </summary>
	/// <typeparam name="TEnumerable">The type of collection the constraint is for</typeparam>
	/// <typeparam name="TValue">The expected value type</typeparam>
	public abstract class EnumerableConstraint<TEnumerable, TValue> : INegatableConstraint where TEnumerable : IEnumerable {

		protected EnumerableConstraint(TEnumerable enumerable, TValue value) {
			Enumerable = enumerable;
			Value = value;
		}

		/// <inheritdoc/>
		public abstract bool Validate();

		public abstract string FailMessage { get; }
		public abstract string NegatedFailMessage { get; }

		/// <summary>
		/// The enumerable to operate on
		/// </summary>
		protected TEnumerable Enumerable { get; private set; }

		/// <summary>
		/// The expected value
		/// </summary>
		protected TValue Value { get; private set; }

	}
}