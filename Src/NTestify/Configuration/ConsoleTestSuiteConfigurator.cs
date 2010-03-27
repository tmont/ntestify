using System;

namespace NTestify.Configuration {
	public class ConsoleTestSuiteConfigurator : ITestConfigurator {
		public void Configure(ITest test) {
			if (test is ITestSuite) {
				test.OnBeforeRun += context => Console.WriteLine("Suite: " + test.Name);
			}
		}
	}
}