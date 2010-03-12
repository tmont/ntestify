using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Configuration;

namespace NTestify.Execution {
	/// <summary>
	/// Accumulates all tests within a namespace
	/// </summary>
	public class NamespaceAccumulator : ITestAccumulator<Type> {
		/// <summary>
		/// Gets an enumeration of class suites and unattaced test methods contained within
		/// a namespace
		/// </summary>
		/// <param name="typeContainedInNamespace">A type contained within the desired namespace</param>
		/// <param name="filters">Accumulation filters</param>
		/// <param name="configurator">Test configurator</param>
		public IEnumerable<ITest> Accumulate(Type typeContainedInNamespace, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			var ns = typeContainedInNamespace.Namespace;
			if (ns == null) {
				//e.g. anonymous type
				throw new ArgumentException("Type given is not contained within a namespace", "typeContainedInNamespace");
			}

			return typeContainedInNamespace
				.Assembly
				.GetTestClasses()
				.Where(type => type.Namespace == ns)
				.Select(type => new ClassSuite(type, filters, configurator).Configure(configurator))
				.Cast<ITest>()
				.Concat(new UnattachedMethodAccumulator()
					.Accumulate(typeContainedInNamespace.Assembly, filters, configurator)
					.Where(test => ((ITestMethodInfo)test).Method.DeclaringType.Namespace == ns)
				);
		}
	}
}