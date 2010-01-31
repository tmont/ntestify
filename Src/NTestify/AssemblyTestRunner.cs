using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NTestify.Logging;

namespace NTestify {

	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods. This is the default test runner in NTestify.
	/// </summary>
	public class AssemblyTestRunner : ITestRunner, ILoggable<AssemblyTestRunner> {
		private ILogger logger;

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		/// <param name="assembly">The assembly in which to search for tests</param>
		public virtual ITestResult RunAll(_Assembly assembly) {
			//create test suites out of each test class
			var classSuites = (
				from type in assembly.GetTestClasses()
				let instance = Activator.CreateInstance(type)
				let innerTests = type
					.GetTestMethods()
					.Select(methodInfo => new ReflectedTestMethod(methodInfo, instance) { Logger = Logger })
					.Cast<ITest>()
				select new TestSuite(innerTests) { Name = instance.GetType().Name, Logger = Logger })
				.ToList();

			//create free floating tests out of test methods that aren't part of a test class
			var freeFloatingTestMethods = (
				from method in assembly.GetUnattachedTestMethods()
				let instance = Activator.CreateInstance(method.DeclaringType)
				select new ReflectedTestMethod(method, instance) { Logger = Logger }).ToList();

			var assemblySuite = new TestSuite { Name = assembly.GetName().Name, Logger = Logger }
				.AddTests(classSuites.Cast<ITest>())
				.AddTests(freeFloatingTestMethods.Cast<ITest>());

			return ((ITestRunner)this).RunTest(assemblySuite, new ExecutionContext { Test = assemblySuite });
		}

		///<inheritdoc/>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
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