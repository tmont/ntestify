using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTestify.Configuration;

namespace NTestify.Execution {
	public class AssemblyAccumulator : ITestAccumulator<_Assembly> {
		public IEnumerable<ITest> Accumulate(_Assembly assembly, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			var namespaceAccumulator = new NamespaceAccumulator();
			var allTypes = assembly.GetTypes();

			var types = new List<Type>();
			foreach (var ns in allTypes.Select(type => type.Namespace).Distinct()) {
				var temp = ns;
				types.Add(allTypes.Where(type => type.Namespace == temp).First());
			}

			return types
				.SelectMany(type => namespaceAccumulator.Accumulate(type, filters, configurator));
		}
	}
}