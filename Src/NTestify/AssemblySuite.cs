using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTestify.Configuration;
using NTestify.Execution;

namespace NTestify {
	/// <summary>
	/// Suite containing all tests within an assembly, organized by namespace
	/// and class
	/// </summary>
	public class AssemblySuite : TestSuite, IAccumulatable<_Assembly, ITestAccumulator<_Assembly>> {
		private readonly ITestAccumulator<_Assembly> accumulator = new AssemblyAccumulator();
		public ITestAccumulator<_Assembly> Accumulator { get { return accumulator; } }

		public _Assembly Assembly { get; protected set; }

		public AssemblySuite(_Assembly assembly) : this(assembly, Enumerable.Empty<IAccumulationFilter>(), new NullConfigurator()) { }

		public AssemblySuite(_Assembly assembly, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			Assembly = assembly;
			Name = assembly.GetName().Name;
			AddTests(Accumulator.Accumulate(assembly, filters, configurator));
		}
	}
}