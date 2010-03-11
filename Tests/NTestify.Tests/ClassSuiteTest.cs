using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class ClassSuiteTest {

		[SetUp]
		public void Setup() {
			SuiteTest.Setup = 0;
			SuiteTest.TearDown = 0;
			BeforeSuiteFilter.Executions = 0;
			AfterSuiteFilter.Executions = 0;
		}

		[TestMethod]
		public void Should_call_suite_setup_and_teardown() {
			var type = typeof(SuiteTest);
			var suite = new ClassSuite(type);

			var context = new ExecutionContext { Instance = new SuiteTest() };
			suite.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Pass));

			Ass.That(SuiteTest.Setup, Is.EqualTo(1));
			Ass.That(SuiteTest.TearDown, Is.EqualTo(1));
		}

		[TestMethod]
		public void Should_execute_filters() {
			var type = typeof(SuiteTest);
			var suite = new ClassSuite(type);

			var context = new ExecutionContext { Instance = new SuiteTest() };
			suite.Run(context);

			Ass.That(context.Result.Status, Is.EqualTo(TestStatus.Pass));
			Ass.That(BeforeSuiteFilter.Executions, Is.EqualTo(1));
			Ass.That(AfterSuiteFilter.Executions, Is.EqualTo(1));
		}

		[BeforeSuiteFilter, AfterSuiteFilter]
		internal class SuiteTest {

			public static int Setup;
			public static int TearDown;

			[SuiteSetup]
			public void SuiteSetup() {
				Setup++;
			}

			[SuiteTearDown]
			public void SuiteTearDown() {
				TearDown++;
			}

		}

		[PreSuiteFilter]
		internal class BeforeSuiteFilter : TestFilter {
			public static int Executions;

			public override void Execute(ExecutionContext executionContext) {
				Executions++;
			}
		}

		[PostSuiteFilter]
		internal class AfterSuiteFilter : TestFilter {
			public static int Executions;

			public override void Execute(ExecutionContext executionContext) {
				Executions++;
			}
		}

	}
}
