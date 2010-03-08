using System;

namespace NTestify.Configuration {
	public class VerboseTestMethodConfigurator : ITestConfigurator {
		public void Configure(ITest test) {
			test.OnBeforeRun += context => Console.Write(test.Name + ": ");
			test.OnError += context => Console.Write("ERROR ");
			test.OnPass += context => Console.Write("PASS ");
			test.OnFail += context => Console.Write("FAIL ");
			test.OnIgnore += context => Console.Write("IGNORE ");
			test.OnAfterRun += context => {
				Console.WriteLine("(" + context.Result.ExecutionTimeInSeconds + ")");
				if (!string.IsNullOrEmpty(context.Result.Message)) {
					Console.WriteLine("  " + context.Result.Message);
				}
				Console.WriteLine(new string('-', 60));
			};
		}
	}
}