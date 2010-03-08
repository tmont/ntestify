using System.Collections.Generic;
using System.Linq;

namespace NTestify.Execution {
	public static class AccumulatorExtensions {
		public static bool Apply(this IEnumerable<IAccumulationFilter> filters, ITest test) {
			return filters.All(filter => filter.Filter(test));
		}
	}
}