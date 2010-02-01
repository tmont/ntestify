using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods. This is the default test runner in NTestify.
	/// </summary>
	public class AssemblyTestRunner : ITestRunner, ILoggable {
		private ILogger logger;
		private ITestConfigurator testSuiteConfigurator;
		private ITestConfigurator testMethodConfigurator;

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		/// <param name="assembly">The assembly in which to search for tests</param>
		public virtual ITestResult RunAll(_Assembly assembly) {
			var classSuites = GetClassSuites(assembly);
			var unattachedTestMethods = GeUnattachedTestMethods(assembly);
			var assemblySuite = new TestSuite { Name = assembly.GetName().Name, Logger = Logger, Configurator = TestSuiteConfigurator }
				.AddTests(classSuites.Cast<ITest>())
				.AddTests(unattachedTestMethods.Cast<ITest>());

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
				select new ReflectedTestMethod(method, instance) { Logger = Logger, Configurator = TestMethodConfigurator }).ToList();
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
					.Select(methodInfo => new ReflectedTestMethod(methodInfo, instance) { Logger = Logger, Configurator = TestMethodConfigurator })
					.Cast<ITest>()
				select new TestSuite(innerTests) { Name = instance.GetType().Name, Logger = Logger, Configurator = TestSuiteConfigurator })
			.ToList();
		}

		///<inheritdoc/>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
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