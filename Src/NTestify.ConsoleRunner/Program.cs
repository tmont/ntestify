using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NTestify.Logging;
using NDesk.Options;

namespace NTestify.ConsoleRunner {
	class Program {
		static void Main(string[] args) {
			string dllPath = null;
			string loggerName = null;
			string loggerConfig = null;
			bool showHelp = false;
			var options = new OptionSet {
				{ "dll=", "The {PATH} to the dll or eexe to scan for tests", v => dllPath = v},
				{"logger-config=", "The {PATH} to the log4net config file to use", v => loggerConfig = v},
				{"logger-name=", "The {NAME} of the logger to use", v => loggerName = v},
				{"h|help|usage", "Show usage", v => showHelp = (v != null)}
			};

			try {
				options.Parse(args);
			} catch (OptionException e) {
				ShowError("Argument error: {0}", e.Message);
				return;
			}

			if (showHelp) {
				ShowUsage(options);
				return;
			}

			if (string.IsNullOrEmpty(dllPath)) {
				ShowError("dll cannot be empty");
				return;
			}

			var assembly = Assembly.LoadFrom(dllPath);

			ILogger logger = null;
			if (loggerName != null) {
				if (loggerName == "console") {
					Console.WindowWidth = Console.LargestWindowWidth - 10;
				}

				logger = new Log4NetLogger(new FileInfo(loggerConfig ?? "log4net.xml"), loggerName);
			}

			var runner = new AssemblyTestRunner {
				Logger = logger,
				TestMethodConfigurator = new ConsoleTestMethodConfigurator(),
				TestSuiteConfigurator = new ConsoleTestSuiteConfigurator()
			};

			PrintVersionString();
			runner.RunAll(assembly);
		}

		private static void ShowError(string message) {
			Console.WriteLine(message);
			Console.ReadLine();
		}

		private static void ShowError(string messageFormat, params object[] args) {
			Console.WriteLine(messageFormat, args);
			Console.ReadLine();
		}

		private static void PrintVersionString() {
			var assembly = Assembly.GetExecutingAssembly();
			var name = assembly.GetName();

			Console.WriteLine(name.Name + " " + name.Version + " by " + assembly.GetAttributes<AssemblyCompanyAttribute>().First().Company);
		}

		private static void ShowUsage(OptionSet options) {
			Console.WriteLine("NTestify Console Runner");
			Console.WriteLine("  (c) 2010 Tommy Montgomery");
			Console.WriteLine();

			options.WriteOptionDescriptions(Console.Out);
		}
	}
}
