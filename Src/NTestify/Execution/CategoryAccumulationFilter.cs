namespace NTestify.Execution {
	public class CategoryAccumulationFilter : RegexAccumulationFilter {
		public override bool Filter(ITest test) {
			return !(test is ReflectedTestMethod) || Regex.IsMatch(test.Category ?? string.Empty);
		}
	}

	public class SuiteCategoryAccumulationFilter : RegexAccumulationFilter {
		public override bool Filter(ITest test) {
			return !(test is ITestSuite) || Regex.IsMatch(test.Category ?? string.Empty);
		}
	}
}