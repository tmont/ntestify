namespace NTestify.Execution {
	public class NamespaceAccumulationFilter : RegexAccumulationFilter {
		public override bool Filter(ITest test) {
			return Regex.IsMatch(test.GetType().Namespace);
		}
	}
}