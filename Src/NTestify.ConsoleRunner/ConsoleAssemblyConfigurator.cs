using System;
using NTestify.Configuration;

namespace NTestify.ConsoleRunner {
	public class ConsoleAssemblyConfigurator : VariableVerbosityConfigurator<AssemblySuite> {
		public ConsoleAssemblyConfigurator(VerbosityLevel verbosityLevel) : base(verbosityLevel) { }

		protected override void ConfigureTest(AssemblySuite test) {
			if (VerbosityLevel > VerbosityLevel.Default) {
				var resultWriter = new PlaintextSuiteResultWriter(new PlaintextMethodResultWriter(VerbosityLevel), VerbosityLevel);
				test.OnBeforeRun += context => Console.WriteLine("Running tests from assembly {0}", test.Assembly.GetName().Name);
				test.OnAfterRun += context => resultWriter.Write(Console.Out, (TestSuiteResult)context.Result);
			}
		}
	}

	public class ConsoleMultipleAssemblyConfigurator : VariableVerbosityConfigurator<ConsoleTestSuite> {
		public ConsoleMultipleAssemblyConfigurator(VerbosityLevel verbosityLevel) : base(verbosityLevel) { }

		protected override void ConfigureTest(ConsoleTestSuite test) {
			if (VerbosityLevel > VerbosityLevel.Minimal) {
				var resultWriter = new PlaintextSuiteResultWriter(new PlaintextMethodResultWriter(VerbosityLevel), VerbosityLevel);
				test.OnAfterRun += context => resultWriter.Write(Console.Out, (TestSuiteResult)context.Result);
			}
		}
	}
}