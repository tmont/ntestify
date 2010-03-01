using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTestify.Configuration;
using NTestify.Logging;

namespace NTestify.Execution {
	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods. This is the default test runner in NTestify.
	/// </summary>
	public class AssemblyTestRunner : ITestRunner, ILoggable {
		private ILogger logger;
		private ITestConfigurator testSuiteConfigurator;
		private ITestConfigurator testMethodConfigurator;

		private readonly IList<IAccumulationFilter> filters;

		public AssemblyTestRunner() {
			filters = new List<IAccumulationFilter>();
		}

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		/// <param name="assembly">The assembly in which to search for tests</param>
		public virtual ITestResult RunAll(_Assembly assembly) {
			var classSuites = GetClassSuites(assembly);
			var unattachedTestMethods = GeUnattachedTestMethods(assembly);
			var assemblySuite = new TestSuite { Name = assembly.GetName().Name, Logger = Logger }
				.AddTests(classSuites.Cast<ITest>())
				.AddTests(unattachedTestMethods.Cast<ITest>())
				.Configure(TestSuiteConfigurator);

			return ((ITestRunner)this).RunTest(assemblySuite, new ExecutionContext { Test = assemblySuite });
		}

		/// <summary>
		/// Gets a list of test methods which aren't part of a class in the assembly
		/// annotated with [Test]
		/// </summary>
		private IEnumerable<ReflectedTestMethod> GeUnattachedTestMethods(_Assembly assembly) {
			return (
				from method in assembly.GetUnattachedTestMethods()
				let instance = Activator.CreateInstance(method.DeclaringType)
				select new ReflectedTestMethod(method, instance) { Logger = Logger }.Configure(TestMethodConfigurator)
				).Where(ApplyFilters).Cast<ReflectedTestMethod>();
		}

		/// <summary>
		/// Gets a list of suites created from classes in the assembly annotated with
		/// [Test]
		/// </summary>
		private IEnumerable<TestSuite> GetClassSuites(_Assembly assembly) {
			return (
				from type in assembly.GetTestClasses()
				let instance = Activator.CreateInstance(type)
				let innerTests = type
					.GetTestMethods()
					.Select(methodInfo => new ReflectedTestMethod(methodInfo, instance) { Logger = Logger }.Configure(TestMethodConfigurator))
					.Where(ApplyFilters)
					.Cast<ITest>()
				select new TestSuite(innerTests) { Name = instance.GetType().Name, Logger = Logger }.Configure(TestSuiteConfigurator)
				).Where(ApplyFilters).Cast<TestSuite>();
		}

		/// <summary>
		/// Runs a single test or suite and returns a result
		/// </summary>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
		}

		private bool ApplyFilters(ITest test) {
			return Filters.All(f => f.Filter(test));
		}

		/// <summary>
		/// Gets all of the accumulation filters for this test runner
		/// </summary>
		public IEnumerable<IAccumulationFilter> Filters {
			get { return filters; }
		}

		/// <summary>
		/// Adds an accumulation filter to the test runner
		/// </summary>
		public ITestRunner AddFilter(IAccumulationFilter filter) {
			filters.Add(filter);
			return this;
		}

		/// <summary>
		/// Gets or sets the configurator to use for test suites
		/// </summary>
		public ITestConfigurator TestSuiteConfigurator {
			get { return testSuiteConfigurator ?? (testSuiteConfigurator = new NullConfigurator()); }
			set { testSuiteConfigurator = value; }
		}

		/// <summary>
		/// Gets or sets the configurator to use for test methods
		/// </summary>
		public ITestConfigurator TestMethodConfigurator {
			get { return testMethodConfigurator ?? (testMethodConfigurator = new NullConfigurator()); }
			set { testMethodConfigurator = value; }
		}

		/// <summary>
		/// Gets or sets the logger used by this test runner
		/// </summary>
		public ILogger Logger {
			get { return logger ?? (logger = new NullLogger()); }
			set { logger = value; }
		}
	}
}