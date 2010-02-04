namespace NTestify.Constraint {
	public interface INegatableConstraint : IConstraint {
		/// <summary>
		/// The negated version of FailMessage; useful to NotConstraint
		/// </summary>
		string NegatedFailMessage { get; }
	}
}