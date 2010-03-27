using System;

namespace NTestify.Configuration {

	public enum VerbosityLevel {
		Minimal = 1,
		Default = 2,
		Verbose = 3
	}

	public abstract class VariableVerbosityConfigurator<TTest> : ITestConfigurator{
		public VerbosityLevel VerbosityLevel { get; private set; }

		protected VariableVerbosityConfigurator() : this(VerbosityLevel.Default) { }

		protected VariableVerbosityConfigurator(VerbosityLevel verbosityLevel) {
			VerbosityLevel = verbosityLevel;
		}

		public void Configure(ITest test){
			if (test is TTest) {
				ConfigureTest((TTest)test);
			}
		}

		protected abstract void ConfigureTest(TTest test);
	}

	public class ConsoleAssemblyConfigurator : VariableVerbosityConfigurator<AssemblySuite> {
		public ConsoleAssemblyConfigurator(VerbosityLevel verbosityLevel) : base(verbosityLevel) { }

		protected override void ConfigureTest(AssemblySuite test) {
			var resultWriter = new PlaintextSuiteResultWriter(new PlaintextMethodResultWriter(VerbosityLevel), VerbosityLevel);
			test.OnAfterRun += context => resultWriter.Write(Console.Out, (TestSuiteResult)context.Result);
		}
	}

	public class ConsoleRunnerConfigurator : ITestConfigurator {
		private readonly VerbosityLevel verbosityLevel;
		private VariableVerbosityConfigurator<ReflectedTestMethod> testMethodConfigurator = new ConsoleTestMethodConfigurator();

		public ConsoleRunnerConfigurator(VerbosityLevel verbosityLevel) {
			this.verbosityLevel = verbosityLevel;
		}

		public VariableVerbosityConfigurator<AssemblySuite> AssemblyConfigurator { get; set; }

		public VariableVerbosityConfigurator<ReflectedTestMethod> TestMethodConfigurator {
			get { return testMethodConfigurator; }
			set { testMethodConfigurator = value; }
		}

		public void Configure(ITest test) {
			if (test is AssemblySuite) {
				(AssemblyConfigurator ?? new ConsoleAssemblyConfigurator(verbosityLevel)).Configure(test);
			} else if (test is ReflectedTestMethod) {
				TestMethodConfigurator.Configure((ReflectedTestMethod)test);
			}
		}
	}

}