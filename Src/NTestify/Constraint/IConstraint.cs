namespace NTestify.Constraint {

	public interface IConstraint<T> {
		bool Validate();
		string FailMessage { get; }
	}

	public interface IConstraint : IConstraint<object> {

	}
}