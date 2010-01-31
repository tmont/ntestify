using System;
using System.Reflection;

namespace NTestify.ConsoleRunner {
	class Program {
		static void Main(string[] args) {
			if (args.Length != 1) {
				Console.Error.WriteLine("Missing first argument of path to DLL to test");
				Console.ReadLine();
				Environment.Exit(1);
			}

			string dllPath = args[0];
			var assembly = Assembly.LoadFrom(dllPath);

			var runner = new AssemblyTestRunner();
			var result = runner.RunAll(assembly);
			Console.WriteLine(result);
			Console.ReadLine();
		}
	}
}
