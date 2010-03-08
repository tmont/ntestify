using System;
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

		public ClassSuite(Type @class) {
			Class = @class;
			AddTests(Accumulator.Accumulate(Class, Enumerable.Empty<IAccumulationFilter>(), new NullConfigurator()));
		}

		/// <summary>
		/// Finds and executes each of the pre suite filters in order, including any SuiteSetup filters
		/// attached to the class, if there are any
		/// </summary>
		/// <exception cref="Test.TestIgnoredException">If the class is annotated with [Ignore]</exception>
		protected override void RunPreTestFilters(ExecutionContext executionContext) {
			var filters = Class
				.GetAttributes<TestFilter>()
				.Where(a => a.GetType().HasAttribute<PreSuiteFilter>());
			if (filters.Any(attribute => attribute is IgnoreAttribute)) {
				//if the suite should be ignored, then we bail immediately without executing any other filters
				throw new TestIgnoredException(filters.Where(filter => filter is IgnoreAttribute).Cast<IgnoreAttribute>().First().Reason);
			}

			filters = filters.Concat(GetInvokableFilters<SuiteSetupAttribute>(Class));
			RunFiltersInOrder(filters, executionContext);
		}

		/// <summary>
		/// Finds and executes each of the post suite filters
		/// </summary>
		protected override void RunPostTestFilters(ExecutionContext executionContext) {
			var filters = Class
				.GetAttributes<TestFilter>()
				.Where(a => a.GetType().HasAttribute<PostSuiteFilter>())
				.Concat(GetInvokableFilters<SuiteTearDownAttribute>(Class));
			RunFiltersInOrder(filters, executionContext);
		}

	}
}