using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NTestify.Execution;
using NTestify.Logging;
using NDesk.Options;

namespace NTestify.ConsoleRunner {
	class Program {
		static void Main(string[] args) {
			string dllPath = null;
			string loggerName = null;
			string loggerConfig = null;
			bool showHelp = false;
			string name = null;
			string suiteName = null;
			string category = null;
			string suiteCategory = null;
			var options = new OptionSet {
				{ "dll=", "The {PATH} to the dll or exe to scan for tests", v => dllPath = v},
				{"logger-config=", "The {PATH} to the log4net config file to use", v => loggerConfig = v},
				{"logger-name=", "The {NAME} of the logger to use", v => loggerName = v},
				{"h|help|usage", "Show usage", v => showHelp = (v != null)},
				{"name=", "{REGEX} to filter test methods by name", v => name = v},
				{"category=", "{REGEX} to filter test methods by category", v => category = v},
				{"suite-category=", "{REGEX} to filter test suites by category", v => suiteCategory = v},
				{"suite-name=", "{REGEX} to filter test suites by name", v => suiteName = v}
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
				TestMethodConfigurator = new VerboseTestMethodConfigurator()
			};


			if (!string.IsNullOrEmpty(name)) {
				runner.AddFilter(new NameAccumulationFilter { Pattern = name });
			}
			if (!string.IsNullOrEmpty(category)) {
				runner.AddFilter(new CategoryAccumulationFilter { Pattern = category });
			}
			if (!string.IsNullOrEmpty(suiteName)) {
				runner.AddFilter(new SuiteNameAccumulationFilter { Pattern = suiteName });
			}
			if (!string.IsNullOrEmpty(suiteCategory)) {
				runner.AddFilter(new SuiteCategoryAccumulationFilter { Pattern = suiteCategory });
			}

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
			var assembly = Assembly.GetAssembly(typeof(ITest));
			var name = assembly.GetName();

			Console.WriteLine(name.Name + " " + name.Version + " by " + assembly.GetAttributes<AssemblyCompanyAttribute>().First().Company);
			Console.WriteLine();
		}

		private static void ShowUsage(OptionSet options) {
			Console.WriteLine("NTestify Console Runner");
			Console.WriteLine("  (c) 2010 Tommy Montgomery");
			Console.WriteLine();

			options.WriteOptionDescriptions(Console.Out);
		}
	}
}
