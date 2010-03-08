using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NTestify.Configuration;

namespace NTestify.Execution {
	public class AssemblyAccumulator : ITestAccumulator<_Assembly> {
		public IEnumerable<ITest> Accumulate(_Assembly assembly, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			var classAccumulator = new ClassAccumulator();
			var unattachedTestMethodAccumulator = new UnattachedMethodAccumulator();

			var tests = assembly
				.GetTestClasses()
				.Select(type => new TestSuite(classAccumulator.Accumulate(type, filters, configurator)) { Name = type.Name }.Configure(configurator))
				.Where(filters.Apply)
				.Cast<ITest>()
				.Concat(unattachedTestMethodAccumulator.Accumulate(assembly, filters, configurator));

			var suite = new TestSuite { Name = assembly.GetName().Name }
				.AddTests(tests)
				.Configure(configurator);

			return new[] { suite };
		}
	}
}