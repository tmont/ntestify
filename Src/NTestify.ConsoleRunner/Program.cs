using System;
using System.IO;
using System.Reflection;
using NTestify.Logging;

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

			Console.WindowWidth = Console.LargestWindowWidth;

			var file = new FileInfo(@"log4net.xml");

			var logger = new Logger(file, "tnet");
			logger.Error("OH NOES!!");
			logger.Error("Seriously!");

			var runner = new AssemblyTestRunner();
			runner.SetLogger(logger);
			var result = runner.RunAll(assembly);
			Console.WriteLine(result);
			Console.ReadLine();
		}
	}
}
