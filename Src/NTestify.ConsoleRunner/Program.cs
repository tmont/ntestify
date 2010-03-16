﻿using System;
using System.Linq;
using System.Reflection;
using NTestify.Configuration;
using NTestify.Execution;
using NDesk.Options;

namespace NTestify.ConsoleRunner {
	class Program {
		
		static string dllPath;
		static bool showHelp;
		static string name;
		static string suiteName;
		static string category;
		static string suiteCategory;
		static string namespaceName;
		static readonly OptionSet Options = new OptionSet {
			{ "dll=", "The {PATH} to the dll or exe to scan for tests", v => dllPath = v},
			{"h|help|usage", "Show usage", v => showHelp = (v != null)},
			{"name=", "{REGEX} to filter test methods by name", v => name = v},
			{"category=", "{REGEX} to filter test methods by category", v => category = v},
			{"suite-category=", "{REGEX} to filter test suites by category", v => suiteCategory = v},
			{"suite-name=", "{REGEX} to filter test suites by name", v => suiteName = v},
			{"namespace=", "Run only tests under the given {NAMESPACE}", v => namespaceName = v}
		};


		static void Main(string[] args) {
			try {
				Options.Parse(args);
			} catch (OptionException e) {
				ShowError("Argument error: {0}", e.Message);
				return;
			}

			if (showHelp) {
				ShowUsage(Options);
				return;
			}

			if (string.IsNullOrEmpty(dllPath)) {
				ShowError("dll cannot be empty");
				return;
			}

			var assembly = Assembly.LoadFrom(dllPath);

			var runner = new AssemblyTestRunner {
				Configurator = new ConsoleRunnerConfigurator(VerbosityLevel.Default)
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
			if (!string.IsNullOrEmpty(namespaceName)) {
				runner.AddFilter(new NamespaceAccumulationFilter { Namespace = namespaceName });
			}

			PrintVersionString();
			runner.RunAll(assembly);
		}

		private static void ShowError(string message) {
			Console.WriteLine(message);
		}

		private static void ShowError(string messageFormat, params object[] args) {
			Console.WriteLine(messageFormat, args);
		}

		private static void PrintVersionString() {
			var assembly = Assembly.GetAssembly(typeof(ITest));
			var assemblyName = assembly.GetName();

			Console.WriteLine(assemblyName.Name + " " + assemblyName.Version + " by " + assembly.GetAttributes<AssemblyCompanyAttribute>().First().Company);
			Console.WriteLine();
		}

		private static void ShowUsage(OptionSet options) {
			PrintVersionString();
			options.WriteOptionDescriptions(Console.Out);
		}
	}
}
