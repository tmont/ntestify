namespace NTestify.Execution {
	public class NameAccumulationFilter : RegexAccumulationFilter {
		public override bool Filter(ITest test) {
			return !(test is ReflectedTestMethod) || Regex.IsMatch(test.Name);
		}
	}

	public class SuiteNameAccumulationFilter : RegexAccumulationFilter {
		public override bool Filter(ITest test) {
			return !(test is ITestSuite) || Regex.IsMatch(test.Name);
		}
	}

}