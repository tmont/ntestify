using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NTestify.Execution;

namespace NTestify.ConsoleRunner {
	public class ConsoleTestRunner : TestSuiteRunner {
		private readonly IEnumerable<FileInfo> files;

		public ConsoleTestRunner(IEnumerable<FileInfo> files) {
			this.files = files;
		}

		public override TestSuiteResult RunAll() {
			var tests = new List<ITest>();

			var assAccumulator = new AssemblyAccumulator();
			foreach (var file in files) {
				if (!file.Exists) {
					throw new FileNotFoundException("File not found", file.FullName);
				}

				tests.AddRange(assAccumulator.Accumulate(Assembly.LoadFrom(file.FullName), Filters, Configurator));
			}

			var suite = new ConsoleTestSuite(files);
			suite.AddTests(tests);
			suite.Configure(Configurator);

			return RunTest(suite);
		}

	}
}