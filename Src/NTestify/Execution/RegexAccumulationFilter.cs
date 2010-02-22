using System.Text.RegularExpressions;

namespace NTestify.Execution {
	public abstract class RegexAccumulationFilter : IAccumulationFilter {
		protected RegexAccumulationFilter() {
			Regex = new Regex(".*");
		}

		protected Regex Regex { get; private set; }

		public string Pattern {
			set { Regex = new Regex(value); }
		}

		public abstract bool Filter(ITest test);
	}
}