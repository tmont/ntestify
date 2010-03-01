namespace NTestify.Configuration {
	/// <summary>
	/// Configurator that does nothing. Its only purpose is to ease
	/// the annoynace of null references.
	/// </summary>
	internal sealed class NullConfigurator : ITestConfigurator {
		public void Configure(ITest test) { }
	}
}