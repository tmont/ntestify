using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTestify.Configuration;

namespace NTestify.Execution {
	public class UnattachedMethodAccumulator : ITestAccumulator<_Assembly> {
		public IEnumerable<ITest> Accumulate(_Assembly assembly, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			return (
				from method in assembly.GetUnattachedTestMethods()
				let instance = Activator.CreateInstance(method.DeclaringType)
				select new ReflectedTestMethod(method, instance).Configure(configurator)
			).Where(filters.Apply).Cast<ITest>();
		}
	}
}