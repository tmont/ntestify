using System.Collections.Generic;
using NTestify.Configuration;

namespace NTestify.Execution {
	public interface ITestAccumulator<TContext> where TContext : class {
		IEnumerable<ITest> Accumulate(TContext context, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator);
	}
}