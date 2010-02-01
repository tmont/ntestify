namespace NTestify.Constraint {
	public abstract class BinaryConstraint<T> : IConstraint {
		protected BinaryConstraint(T expected, T actual) {
			Expected = expected;
			Actual = actual;
		}

		public T Expected { get; private set; }
		public T Actual { get; private set; }

		public abstract bool Validate();
		public abstract string FailMessage { get; }
	}
}