using System;
using System.IO;
using System.Linq;

namespace NTestify.Configuration {
	public interface IResultWriter<TResult> where TResult : ITestResult {
		void Write(TextWriter writer, TResult result);
	}

	public class PlaintextMethodResultWriter : IResultWriter<TestMethodResult> {
		private readonly VerbosityLevel verbosityLevel;
		public PlaintextMethodResultWriter() : this(VerbosityLevel.Default) { }

		public PlaintextMethodResultWriter(VerbosityLevel verbosityLevel) {
			this.verbosityLevel = verbosityLevel;
		}

		public void Write(TextWriter writer, TestMethodResult result) {
			if (verbosityLevel > VerbosityLevel.Minimal && (result.Errors.Any() || result.Status != TestStatus.Pass)) {
				writer.WriteLine(result.Test.Name);

				const string indent = "  ";
				switch (result.Status) {
					case TestStatus.Error:
						foreach (var error in result.Errors) {
							writer.WriteLine(indent + error.Message);
							writer.WriteLine(indent + error.StackTrace.Replace(Environment.NewLine, Environment.NewLine + indent));
							writer.WriteLine();
						}
						break;
					case TestStatus.Fail:
					case TestStatus.Ignore:
						writer.WriteLine(result.Message);
						writer.WriteLine(result.StackTrace);
						break;
				}
			}
		}
	}

	public static class ResultWriterExtensions {
		public static void WriteHorizontalRuler(this TextWriter writer) {
			writer.WriteLine(new string('-', 60));
		}
	}

	public class PlaintextSuiteResultWriter : IResultWriter<TestSuiteResult> {
		private readonly IResultWriter<TestMethodResult> testMethodResultWriter;
		private readonly VerbosityLevel verbosityLevel;

		public PlaintextSuiteResultWriter(IResultWriter<TestMethodResult> testMethodResultWriter) : this(testMethodResultWriter, VerbosityLevel.Default) { }

		public PlaintextSuiteResultWriter(IResultWriter<TestMethodResult> testMethodResultWriter, VerbosityLevel verbosityLevel) {
			this.testMethodResultWriter = testMethodResultWriter;
			this.verbosityLevel = verbosityLevel;
		}

		public void Write(TextWriter writer, TestSuiteResult result) {
			writer.WriteLine();
			if (result.Status != TestStatus.Pass) {
				writer.WriteLine();
			}

			if (verbosityLevel > VerbosityLevel.Minimal) {
				WriteStatusResults(writer, result, TestStatus.Error, "Errors");
				WriteStatusResults(writer, result, TestStatus.Fail, "Failures");
				WriteStatusResults(writer, result, TestStatus.Ignore, "Ignored Tests");
				WriteSummary(writer, result);
			}
		}

		private static void WriteSummary(TextWriter writer, TestSuiteResult result) {
			var count = result.InnerCount;
			var passedTests = result.InnerPassedTests.Count();
			var ignoredTests = result.InnerIgnoredTests.Count();
			var failedTests = result.InnerFailedTests.Count();
			var erredTests = result.InnerErredTests.Count();

			writer.WriteHorizontalRuler();
			writer.WriteLine("Passing tests: {0} ({1}%)", passedTests, Math.Round((double)passedTests / count * 100d, 2));
			writer.WriteLine("Ignored tests: {0} ({1}%)", ignoredTests, Math.Round((double)ignoredTests / count * 100d, 2));
			writer.WriteLine("Erring tests:  {0} ({1}%)", erredTests, Math.Round((double)erredTests / count * 100d, 2));
			writer.WriteLine("Failing tests: {0} ({1}%)", failedTests, Math.Round((double)failedTests / count * 100d, 2));
			writer.WriteHorizontalRuler();
			writer.WriteLine("Total:         {0}", count);
		}

		private void WriteStatusResults(TextWriter writer, TestSuiteResult result, TestStatus status, string header) {
			if (!result.InnerResults.Any(r => r.Status == status)) {
				return;
			}

			writer.WriteLine(header);
			writer.WriteHorizontalRuler();
			var i = 1;
			foreach (var testMethodResult in result.InnerResults.Where(r => r.Status == status).Cast<TestMethodResult>().OrderBy(r => r.StartTime)) {
				writer.Write(i++ + ". ");
				testMethodResultWriter.Write(writer, testMethodResult);
			}

			writer.WriteLine();
		}

	}

}