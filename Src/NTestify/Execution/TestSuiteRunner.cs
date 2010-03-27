using System.Collections.Generic;
using System.Collections.ObjectModel;
using NTestify.Configuration;

namespace NTestify.Execution {
	public abstract class TestSuiteRunner : ITestRunner {
		private readonly ICollection<IAccumulationFilter> filters;

		protected TestSuiteRunner() {
			filters = new Collection<IAccumulationFilter>();
		}

		public abstract TestSuiteResult RunAll();

		protected TestSuiteResult RunTest(ITestSuite suite) {
			return (TestSuiteResult)((ITestRunner)this).RunTest(suite, CreateContext(suite));
		}

		protected virtual ExecutionContext CreateContext(ITestSuite suite) {
			return new ExecutionContext { Test = suite };
		}

		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
		}

		public IEnumerable<IAccumulationFilter> Filters {
			get { return filters; }
		}

		public ITestRunner AddFilter(IAccumulationFilter filter) {
			filters.Add(filter);
			return this;
		}

		public ITestConfigurator Configurator { get; set; }
	}
}