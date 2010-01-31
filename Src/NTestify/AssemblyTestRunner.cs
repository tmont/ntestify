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
					.Select(methodInfo => new ReflectedTestMethod(methodInfo, instance).SetLogger(Logger))
					.Cast<ITest>()
				select new TestSuite(innerTests) { Name = instance.GetType().Name})
				.ToList();

			//create free floating tests out of test methods that aren't part of a test class
			var freeFloatingTestMethods = (
				from method in assembly.GetUnattachedTestMethods()
				let instance = Activator.CreateInstance(method.DeclaringType)
				select new ReflectedTestMethod(method, instance)).ToList();

			//set up logging
			freeFloatingTestMethods.ForEach(method => method.SetLogger(Logger));
			classSuites.ForEach(method => method.SetLogger(Logger));

			var assemblySuite = new TestSuite { Name = assembly.GetName().Name }
				.AddTests(classSuites.Cast<ITest>())
				.AddTests(freeFloatingTestMethods.Cast<ITest>());

			((TestSuite)assemblySuite).SetLogger(Logger);

			return ((ITestRunner)this).RunTest(assemblySuite, new ExecutionContext { Test = assemblySuite });
		}

		///<inheritdoc/>
		ITestResult ITestRunner.RunTest(ITest test, ExecutionContext executionContext) {
			test.Run(executionContext);
			return executionContext.Result;
		}

		/// <summary>
		/// Sets the logger that this test will use. Calling this method
		/// has a chain reaction effect, in that every test found in the assembly
		/// will have this logger attached to it.
		/// </summary>
		/// <param name="logger">The logger to attach to the test. If null, a NullLogger is attached.</param>
		public AssemblyTestRunner SetLogger(ILogger logger){
			if (logger == null) {
				logger = new NullLogger();
			}

			Logger = logger;
			return this;
		}

		/// <summary>
		/// The logger used by this test runner
		/// </summary>
		protected ILogger Logger { get; private set; }
	}
}