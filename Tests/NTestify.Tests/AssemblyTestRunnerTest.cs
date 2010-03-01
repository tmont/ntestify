using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Moq;
using NTestify.Execution;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class AssemblyTestRunnerTest {

		[TestMethod]
		public void Should_run_tests_in_assembly() {
			//this is really an integration test...

			var assembly = new Mock<_Assembly>();

			assembly
				.Setup(a => a.GetTypes())
				.Returns(new[] { typeof(FakeAssemblyTestClass), typeof(FakeAssemblyTestClassThatIsNotATest) });
			assembly
				.Setup(a => a.GetName())
				.Returns(new AssemblyName("A Fake Assembly"));

			var runner = new AssemblyTestRunner();

			var result = runner.RunAll(assembly.Object).CastTo<TestSuiteResult>();
			Ass.That(result.Status, Is.EqualTo(TestStatus.Pass));

			//verify that the tests that got run were the ones we wanted
			Ass.That(result.OuterCount, Is.EqualTo(2)); //one suite, plus one freefloater
			Ass.That(result.InnerCount, Is.EqualTo(2)); //two test methods
			Ass.That(result.InnerTests.First().Name, Is.EqualTo("FakeAssemblyTestClass.FakeTest"));
			Ass.That(result.InnerTests.Last().Name, Is.EqualTo("FakeAssemblyTestClassThatIsNotATest.AnotherFakeTest"));
			Ass.That(result.OuterTests.First().Name, Is.EqualTo("FakeAssemblyTestClass"));
			Ass.That(result.OuterTests.Last().Name, Is.EqualTo("FakeAssemblyTestClassThatIsNotATest.AnotherFakeTest"));
			Ass.That(result.Test.Name, Is.EqualTo("A Fake Assembly"));
		}

	}

	#region Mocks
	[Test]
	internal class FakeAssemblyTestClass {

		[Test]
		public void FakeTest() { }

	}

	internal class FakeAssemblyTestClassThatIsNotATest {
		[Test]
		public void AnotherFakeTest() { }
	}
	#endregion
}
