using System;
using System.Linq;
using System.Reflection;
using NTestify.Configuration;
using NTestify.Execution;

namespace NTestify {
	/// <summary>
	/// A suite representing all tests within a particular namespace
	/// </summary>
	public class NamespaceSuite : TestSuite, IAccumulatable<Type, ITestAccumulator<Type>> {
		private readonly ITestAccumulator<Type> accumulator = new NamespaceAccumulator();

		/// <summary>
		/// Gets the test accumulator
		/// </summary>
		public ITestAccumulator<Type> Accumulator { get { return accumulator; } }

		/// <summary>
		/// The namespace of all the tests this suite contains by default
		/// </summary>
		public string Namespace { get; private set; }

		/// <param name="assembly">The assembly of the namespace</param>
		/// <param name="namespace">The namespace to search</param>
		public NamespaceSuite(Assembly assembly, string @namespace) {
			Namespace = @namespace;
			var typeContainedInNamespace = assembly.GetTypes().FirstOrDefault(type => type.Namespace == Namespace);
			if (typeContainedInNamespace == null) {
				throw new ArgumentException(string.Format("Assembly \"{0}\" contains no types for namespace \"{1}\".", assembly, Namespace), "assembly");
			}

			AddTests(Accumulator.Accumulate(typeContainedInNamespace, Enumerable.Empty<IAccumulationFilter>(), new NullConfigurator()));
		}

	}
}