using System.Runtime.InteropServices;

namespace NTestify.Execution {

	/// <summary>
	/// Test runner that scans an assembly for testable classes
	/// and methods
	/// </summary>
	public class AssemblyTestRunner : TestSuiteRunner {
		private readonly _Assembly assembly;

		public AssemblyTestRunner(_Assembly assembly) {
			this.assembly = assembly;
		}

		/// <summary>
		/// Runs all available tests in the assembly
		/// </summary>
		public override TestSuiteResult RunAll() {
			return RunTest(CreateTest(assembly));
		}

		/// <summary>
		/// Creates the assembly suite
		/// </summary>
		/// <param name="assemblyToScan">The assembly to scan for tests</param>
		protected virtual ITestSuite CreateTest(_Assembly assemblyToScan) {
			var test = new AssemblySuite(assemblyToScan, Filters, Configurator);
			test.Configure(Configurator);
			return test;
		}

	}
}