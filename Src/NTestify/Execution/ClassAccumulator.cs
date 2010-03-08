using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Configuration;

namespace NTestify.Execution {
	/// <summary>
	/// Accumulates all tests within a class
	/// </summary>
	public class ClassAccumulator : ITestAccumulator<Type> {

		/// <summary>
		/// Gets all tests contained within the given type
		/// </summary>
		/// <param name="type">The class to search for test methods</param>
		/// <param name="filters">The accumulation filters</param>
		/// <param name="configurator">The test configurator</param>
		public IEnumerable<ITest> Accumulate(Type type, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			var instance = Activator.CreateInstance(type);

			return type
				.GetTestMethods()
				.Select(methodInfo => new ReflectedTestMethod(methodInfo, instance).Configure(configurator))
				.Where(filters.Apply);
		}

	}
}