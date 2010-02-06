namespace NTestify.Mock.Expression {
	/// <summary>
	/// Represents the callback node in the mock expression tree
	/// </summary>
	/// <typeparam name="T">The type of the mock object</typeparam>
	/// <typeparam name="TReturn">The expected invocation's return value</typeparam>
	public class CallbackExpression<T, TReturn> where T : class {
		private readonly Expectation<T, TReturn> expectation;

		internal CallbackExpression(Expectation<T, TReturn> expectation) {
			this.expectation = expectation;
		}

		/// <summary>
		/// Instructs the expectation that it expects to return these values
		/// upon invocation
		/// </summary>
		public void AndReturn(params TReturn[] returnValues) {
			expectation.AddReturnValues(returnValues);
		}

	}
}