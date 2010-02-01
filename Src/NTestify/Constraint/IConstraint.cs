namespace NTestify.Constraint {

	public interface IConstraint {
		bool Validate();
		string FailMessage { get; }
	}
}