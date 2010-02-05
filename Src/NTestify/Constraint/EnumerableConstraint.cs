using System.Collections;

namespace NTestify.Constraint {
	/// <summary>
	/// Common base class for constraints on collections
	/// </summary>
	/// <typeparam name="T">The type of collection the constraint is for</typeparam>
	public abstract class EnumerableConstraint<T> : INegatableConstraint where T : IEnumerable {

		protected EnumerableConstraint(T enumerable, object value) {
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
		protected T Enumerable { get; private set; }

		/// <summary>
		/// The expected value
		/// </summary>
		protected object Value { get; private set; }

	}
}