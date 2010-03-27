namespace NTestify.Configuration {
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
}