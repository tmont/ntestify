using NTestify.Configuration;

namespace NTestify.ConsoleRunner {
	public class ConsoleRunnerConfigurator : ITestConfigurator {
		public ConsoleRunnerConfigurator(VerbosityLevel verbosityLevel) {
			TestMethodConfigurator = new ConsoleTestMethodConfigurator();
			AssemblyConfigurator = new ConsoleAssemblyConfigurator(verbosityLevel);
			ConsoleSuiteConfigurator = new ConsoleMultipleAssemblyConfigurator(verbosityLevel);
		}

		public VariableVerbosityConfigurator<ConsoleTestSuite> ConsoleSuiteConfigurator { get; set; }
		public VariableVerbosityConfigurator<AssemblySuite> AssemblyConfigurator { get; set; }
		public VariableVerbosityConfigurator<ReflectedTestMethod> TestMethodConfigurator { get; set; }

		public void Configure(ITest test) {
			if (test is ConsoleTestSuite) {
				ConsoleSuiteConfigurator.Configure(test);
			} else if (test is AssemblySuite) {
				AssemblyConfigurator.Configure(test);
			} else if (test is ReflectedTestMethod) {
				TestMethodConfigurator.Configure(test);
			}
		}
	}
}