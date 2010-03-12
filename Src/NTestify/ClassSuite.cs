using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Configuration;
using NTestify.Execution;

namespace NTestify {
	/// <summary>
	/// A test suite containing all test methods within a class
	/// </summary>
	public class ClassSuite : TestSuite, IAccumulatable<Type, ITestAccumulator<Type>> {
		private readonly ClassAccumulator accumulator = new ClassAccumulator();

		/// <summary>
		/// The class that contains the test methods
		/// </summary>
		public Type Class { get; private set; }

		/// <summary>
		/// Gets the test accumulator
		/// </summary>
		public ITestAccumulator<Type> Accumulator { get { return accumulator; } }

		public ClassSuite(Type @class) : this(@class, Enumerable.Empty<IAccumulationFilter>(), new NullConfigurator()) { }

		public ClassSuite(Type @class, IEnumerable<IAccumulationFilter> filters, ITestConfigurator configurator) {
			Class = @class;
			Name = Class.Name;
			AddTests(Accumulator.Accumulate(Class, filters, configurator));
		}

		/// <summary>
		/// Finds and executes each of the pre suite filters in order, including any SuiteSetup filters
		/// attached to the class, if there are any
		/// </summary>
		/// <exception cref="Test.TestIgnoredException">If the class is annotated with [Ignore]</exception>
		protected override void RunPreTestFilters(ExecutionContext executionContext) {
			//check if the entire suite should be ignored first
			if (Class.HasAttribute<IgnoreAttribute>()) {
				throw new TestIgnoredException(Class.GetAttributes<IgnoreAttribute>().First().Reason);
			}

			RunFiltersInOrder(GetFilters<SuiteSetupAttribute, PreSuiteFilter>(), executionContext);
		}

		/// <summary>
		/// Finds and executes each of the post suite filters
		/// </summary>
		protected override void RunPostTestFilters(ExecutionContext executionContext) {
			RunFiltersInOrder(GetFilters<SuiteTearDownAttribute, PostSuiteFilter>(), executionContext);
		}

		private IEnumerable<TestFilter> GetFilters<TInvokableFilter, TSuiteFilter>() where TInvokableFilter : InvokableFilter where TSuiteFilter : Attribute {
			return Class
				.GetInvokableFilters<TInvokableFilter>()
				.Union(Class.GetFilters<TSuiteFilter>());
		}

	}
}