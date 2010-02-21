using System;

namespace NTestify {
	
	/// <summary>
	/// Represents a testable object by the NTestify framework
	/// </summary>
	public interface ITest {
		/// <summary>
		/// Runs the test, and sets the ExecutionContext's Result property
		/// with the result of the test
		/// </summary>
		/// <param name="executionContext">The current test execution context</param>
		void Run(ExecutionContext executionContext);

		/// <summary>
		/// Gets or sets the name of the test
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// Uses the given configurator to configure the test
		/// </summary>
		ITest Configure(ITestConfigurator configurator);

		/// <summary>
		/// Event that fires before a test runs
		/// </summary>
		event Action<ExecutionContext> OnBeforeRun;
		/// <summary>
		/// Event that fires after a test runs
		/// </summary>
		event Action<ExecutionContext> OnAfterRun;
		/// <summary>
		/// Event that fires if a test is ignored
		/// </summary>
		event Action<ExecutionContext> OnIgnore;
		/// <summary>
		/// Event that fires when a test passes
		/// </summary>
		event Action<ExecutionContext> OnPass;
		/// <summary>
		/// Event that fires when a test fails
		/// </summary>
		event Action<ExecutionContext> OnFail;
		/// <summary>
		/// Event that fires when a test errs
		/// </summary>
		event Action<ExecutionContext> OnError;
	}
}