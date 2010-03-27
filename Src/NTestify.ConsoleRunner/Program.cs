using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NTestify.Configuration;
using NTestify.Execution;
using NDesk.Options;

namespace NTestify.ConsoleRunner {
	class Program {

		static bool showHelp;
		static bool printVersion;
		static string name;
		static string suiteName;
		static string category;
		static string suiteCategory;
		static VerbosityLevel verbosityLevel;
		static string namespaceName;
		static IList<string> files;
		static readonly OptionSet Options = new OptionSet {
			{"h|help|usage", "Show usage", v => showHelp = (v != null)},
			{"version", "Print version details and exit", v => printVersion = (v != null)},
			{"name=", "{REGEX} to filter test methods by name", v => name = v},
			{"category=", "{REGEX} to filter test methods by category", v => category = v},
			{"suite-category=", "{REGEX} to filter test suites by category", v => suiteCategory = v},
			{"suite-name=", "{REGEX} to filter test suites by name", v => suiteName = v},
			{"namespace=", "Run only tests under the given {NAMESPACE}", v => namespaceName = v},
			{"v|verbosity=", "One of Minimal, Default or Verbose", v => verbosityLevel = (VerbosityLevel)Enum.Parse(typeof(VerbosityLevel), v ?? "Default", true)}
		};

		static void Main(string[] args) {
			try {
				files = Options.Parse(args);
			} catch (OptionException e) {
				ShowError("Argument error: {0}", e.Message);
				return;
			}

			if (showHelp) {
				ShowUsage();
				return;
			}
			if (printVersion) {
				PrintVersionString();
				return;
			}
			if (!files.Any()) {
				ShowError("Must specify at least one assembly");
				return;
			}

			var runner = new ConsoleTestRunner(files.Select(f => new FileInfo(f))) {
				Configurator = new ConsoleRunnerConfigurator(verbosityLevel)
			};

			AddFilters(runner);

			if (verbosityLevel > VerbosityLevel.Minimal) {
				PrintVersionString();
			}

			runner.RunAll();

			Console.ReadLine();
		}

		private static void AddFilters(ITestRunner runner) {
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
			if (!string.IsNullOrEmpty(namespaceName)) {
				runner.AddFilter(new NamespaceAccumulationFilter { Namespace = namespaceName });
			}
		}

		private static void ShowError(string messageFormat, params object[] args) {
			Console.Error.WriteLine(messageFormat, args);
		}

		private static void PrintVersionString() {
			var assembly = Assembly.GetAssembly(typeof(ITest));
			var assemblyName = assembly.GetName();

			Console.WriteLine(assemblyName.Name + " " + assemblyName.Version + " by " + assembly.GetAttributes<AssemblyCompanyAttribute>().First().Company);
			Console.WriteLine();
		}

		private static void ShowUsage() {
			PrintVersionString();
			Options.WriteOptionDescriptions(Console.Out);
		}
	}
}
