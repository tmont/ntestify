using System;
using System.IO;
using System.Linq;

namespace NTestify.Configuration {

	public enum VerbosityLevel {
		Minimal = 1,
		Default = 2,
		Verbose = 3
	}

	public abstract class VariableVerbosityConfigurator<TTest> : ITestConfigurator where TTest : class, ITest {
		public VerbosityLevel VerbosityLevel { get; private set; }

		protected VariableVerbosityConfigurator() : this(VerbosityLevel.Default) { }

		protected VariableVerbosityConfigurator(VerbosityLevel verbosityLevel) {
			VerbosityLevel = verbosityLevel;
		}

		public void Configure(ITest test) {
			var typedTest = test as TTest;
			if (typedTest == null) {
				throw new ArgumentException("Test must be of type " + typeof(TTest).GetFriendlyName(), "test");
			}

			ConfigureTest(typedTest);
		}

		protected abstract void ConfigureTest(TTest test);
	}

	public class ConsoleAssemblyConfigurator : VariableVerbosityConfigurator<AssemblySuite> {
		public ConsoleAssemblyConfigurator(VerbosityLevel verbosityLevel) : base(verbosityLevel) { }

		protected override void ConfigureTest(AssemblySuite test) {
			var resultWriter = new PlaintextResultWriter(Console.Out, VerbosityLevel);
			if (VerbosityLevel > VerbosityLevel.Minimal) {
				test.OnBeforeRun += context => Console.WriteLine("Running tests in assembly {0}", test.Assembly.GetName().Name);
				test.OnAfterRun += context => resultWriter.Write(context.Result);
			}
		}
	}

	public interface IResultWriter {
		void Write(ITestResult result);
	}

	public class PlaintextResultWriter : IResultWriter {
		public PlaintextResultWriter(TextWriter writer) : this(writer, VerbosityLevel.Default) { }

		public PlaintextResultWriter(TextWriter writer, VerbosityLevel verbosityLevel) {
			Writer = writer;
			VerbosityLevel = verbosityLevel;
		}

		public VerbosityLevel VerbosityLevel { get; private set; }

		public TextWriter Writer { get; private set; }

		public void Write(ITestResult result) {
			if (result is TestSuiteResult) {
				WriteSuiteResult((TestSuiteResult)result);
			} else {
				WriteMethodResult((TestMethodResult)result);
			}
		}

		protected void WriteHorizontalRuler() {
			Writer.WriteLine(new string('-', 60));
		}

		protected virtual void WriteSuiteResult(TestSuiteResult result) {
			Writer.WriteLine();
			if (result.Status != TestStatus.Pass) {
				Writer.WriteLine();
			}

			WriteStatusResults(result, TestStatus.Error, "Errors");
			WriteStatusResults(result, TestStatus.Fail, "Failures");
			WriteStatusResults(result, TestStatus.Ignore, "Ignored Tests");

			WriteSummary(result);
		}

		private void WriteSummary(TestSuiteResult result){
			var count = result.InnerCount;
			var passedTests = result.InnerPassedTests.Count();
			var ignoredTests = result.InnerIgnoredTests.Count();
			var failedTests = result.InnerFailedTests.Count();
			var erredTests = result.InnerErredTests.Count();

			WriteHorizontalRuler();
			Writer.WriteLine("Passing tests: {0} ({1}%)", passedTests, Math.Round((double)passedTests / count * 100d, 2));
			Writer.WriteLine("Ignored tests: {0} ({1}%)", ignoredTests, Math.Round((double)ignoredTests / count * 100d, 2));
			Writer.WriteLine("Erring tests:  {0} ({1}%)", erredTests, Math.Round((double)erredTests / count * 100d, 2));
			Writer.WriteLine("Failing tests: {0} ({1}%)", failedTests, Math.Round((double)failedTests / count * 100d, 2));
			WriteHorizontalRuler();
			Writer.WriteLine("Total:         {0}", count);
		}

		protected void WriteStatusResults(TestSuiteResult result, TestStatus status, string header) {
			if (!result.InnerResults.Any(r => r.Status == status)) {
				return;
			}

			Writer.WriteLine(header);
			WriteHorizontalRuler();
			var i = 1;
			foreach (var test in result.InnerResults.Where(r => r.Status == status).Cast<TestMethodResult>().OrderBy(r => r.StartTime)) {
				Writer.Write(i++ + ". ");
				WriteMethodResult(test);
			}

			Writer.WriteLine();
		}

		protected virtual void WriteMethodResult(TestMethodResult result) {
			if (VerbosityLevel > VerbosityLevel.Minimal && (result.Errors.Any() || result.Status != TestStatus.Pass)) {
				Writer.WriteLine(result.Test.Name);

				const string indent = "  ";
				switch (result.Status) {
					case TestStatus.Error:
						foreach (var error in result.Errors) {
							Writer.WriteLine(indent + error.Message);
							Writer.WriteLine(indent + error.StackTrace.Replace(Environment.NewLine, Environment.NewLine + indent));
							Writer.WriteLine();
						}
						break;
					case TestStatus.Fail:
					case TestStatus.Ignore:
						Writer.WriteLine(result.Message);
						break;
				}
			}
		}
	}

	public class ConsoleRunnerConfigurator : ITestConfigurator {
		private readonly VerbosityLevel verbosityLevel;
		private VariableVerbosityConfigurator<ReflectedTestMethod> testMethodConfigurator = new ConsoleTestMethodConfigurator();

		public ConsoleRunnerConfigurator(VerbosityLevel verbosityLevel) {
			this.verbosityLevel = verbosityLevel;
		}

		public VariableVerbosityConfigurator<AssemblySuite> AssemblyConfigurator { get; set; }
		public VariableVerbosityConfigurator<NamespaceSuite> NamespaceConfigurator { get; set; }
		
		public VariableVerbosityConfigurator<ReflectedTestMethod> TestMethodConfigurator{
			get { return testMethodConfigurator; }
			set { testMethodConfigurator = value; }
		}

		public void Configure(ITest test) {
			if (test is AssemblySuite) {
				(AssemblyConfigurator ?? new ConsoleAssemblyConfigurator(verbosityLevel)).Configure(test);
			} else if (test is NamespaceSuite) {
				//(NamespaceConfigurator ?? new ConsoleNamespaceConfigurator(verbosityLevel)).Configure(test);
			} else if (test is ClassSuite) {

			} else if (test is ITestMethodInfo) {
				TestMethodConfigurator.Configure(test);
			}
		}
	}

	public class AssemblyConfigurator : ITestConfigurator {
		private readonly ITestConfigurator suiteConfigurator;
		private readonly ITestConfigurator methodConfigurator;

		public AssemblyConfigurator() : this(new ConsoleTestSuiteConfigurator(), new ConsoleTestMethodConfigurator()) { }

		public AssemblyConfigurator(ITestConfigurator suiteConfigurator, ITestConfigurator methodConfigurator) {
			this.suiteConfigurator = suiteConfigurator;
			this.methodConfigurator = methodConfigurator;
		}

		public void Configure(ITest test) {
			if (test is ITestSuite) {
				ConfigureTestSuite((ITestSuite)test);
			} else {
				ConfigureTestMethod(test);
			}
		}

		protected virtual void ConfigureTestMethod(ITest test) {
			suiteConfigurator.Configure(test);
		}

		protected virtual void ConfigureTestSuite(ITestSuite suite) {
			methodConfigurator.Configure(suite);
		}
	}
}