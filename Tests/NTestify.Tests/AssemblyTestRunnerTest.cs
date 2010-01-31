﻿using System.Linq;
using System.Runtime.InteropServices;
using Moq;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class AssemblyTestRunnerTest {

		[TestMethod]
		public void Should_do_something() {
			//this is really an integration test...

			var assembly = new Mock<_Assembly>();
			assembly
				.Setup(a => a.GetTypes())
				.Returns(new[] { typeof(FakeAssemblyTestClass), typeof(FakeAssemblyTestClassThatIsNotATest) });
			assembly
				.SetupGet(a => a.FullName)
				.Returns("A Fake Assembly");

			var runner = new AssemblyTestRunner();

			var result = runner.RunAll(assembly.Object).CastTo<TestSuiteResult>();
			Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));

			//verify that the tests that got run were the ones we wanted
			Assert.That(result.OuterCount, Is.EqualTo(2)); //one suite, plus one freefloater
			Assert.That(result.InnerCount, Is.EqualTo(2)); //two test methods
			Assert.That(result.InnerTests.First().Name, Is.EqualTo("FakeAssemblyTestClass.FakeTest"));
			Assert.That(result.InnerTests.Last().Name, Is.EqualTo("FakeAssemblyTestClassThatIsNotATest.AnotherFakeTest"));
			Assert.That(result.OuterTests.First().Name, Is.EqualTo("FakeAssemblyTestClass"));
			Assert.That(result.OuterTests.Last().Name, Is.EqualTo("FakeAssemblyTestClassThatIsNotATest.AnotherFakeTest"));
			Assert.That(result.Test.Name, Is.EqualTo("A Fake Assembly"));
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