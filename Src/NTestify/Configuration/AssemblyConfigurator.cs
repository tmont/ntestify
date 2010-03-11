namespace NTestify.Configuration {
	public class AssemblyConfigurator : ITestConfigurator {
		private readonly ITestConfigurator suiteConfigurator;
		private readonly ITestConfigurator methodConfigurator;

		public AssemblyConfigurator() : this(new ConsoleTestSuiteConfigurator(), new ConsoleTestMethodConfigurator()) { }

		public AssemblyConfigurator(ITestConfigurator suiteConfigurator, ITestConfigurator methodConfigurator) {
			this.suiteConfigurator = suiteConfigurator;
			this.methodConfigurator = methodConfigurator;
		}

		public void Configure(ITest test) {
			if (test is ITestSuite) {
				ConfigureTestSuite((ITestSuite)test);
			} else {
				ConfigureTestMethod(test);
			}
		}

		protected virtual void ConfigureTestMethod(ITest test) {
			suiteConfigurator.Configure(test);
		}

		protected virtual void ConfigureTestSuite(ITestSuite suite) {
			methodConfigurator.Configure(suite);
		}
	}
}