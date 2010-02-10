namespace NTestify.Mock.Expressions {
	/// <summary>
	/// Represents the invocation count node in the mock expression
	/// </summary>
	/// <typeparam name="T">The mock object's type</typeparam>
	/// <typeparam name="TReturn">The expected invocation's return value type</typeparam>
	public class InvocationCountExpression<T, TReturn> where T : class {
		private readonly Expectation<T, TReturn> expectation;

		internal InvocationCountExpression(Expectation<T, TReturn> expectation){
			this.expectation = expectation;
		}

		/// <summary>
		/// Instructs the expectation that it is expected to return
		/// these values
		/// </summary>
		/// <param name="returnValues">The expected return values</param>
		public void AndReturn(params TReturn[] returnValues) {
			expectation.AddReturnValues(returnValues);
		}
	}
}