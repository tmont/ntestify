namespace NTestify {
	/// <summary>
	/// Represents the result of a completed test method
	/// </summary>
	public class TestMethodResult : TestResult<ReflectedTestMethod> {
		public TestMethodResult(ReflectedTestMethod test) : base(test) { }
	}

}