using System.Linq;
using Moq;
using NTestify.Configuration;
using NTestify.Execution;
using NTestify.Tests.AccumulationHelper;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests.AccumulationHelper {
	[Test]
	internal class Foo {
		[Test]
		public void FooTest() { }
	}

	[Test, Ignore]
	internal class IgnoredTest { }

	internal class Bar {
		[Test]
		public void BarTest() { }

		public void ShouldNotBeAccumulated() { }
	}
}

namespace NTestify.Tests {
	[TestFixture]
	public class AccumulationTests {

		[TestMethod]
		public void Should_accumulate_all_tests_in_class() {
			var configurator = new Mock<ITestConfigurator>();
			configurator.Setup(c => c.Configure(It.IsAny<ReflectedTestMethod>())).Verifiable();
			var tests = new ClassAccumulator().Accumulate(typeof(FakeTestClass), Enumerable.Empty<IAccumulationFilter>(), configurator.Object);

			Ass.That(tests, Is.Not.Empty);
			Ass.That(tests.Count(), Is.EqualTo(2));
			Ass.That(tests.All(test => test is ReflectedTestMethod), "All tests should be ReflectedTestMethod");
			Ass.That(tests.First().Name, Is.EqualTo("FakeTestClass.Test1"));
			Ass.That(tests.Last().Name, Is.EqualTo("FakeTestClass.Test2"));
			configurator.VerifyAll();
		}

		[TestMethod]
		public void Should_accumulate_all_tests_in_namespace() {
			var tests = new NamespaceAccumulator().Accumulate(typeof(Foo), Enumerable.Empty<IAccumulationFilter>(), new NullConfigurator());

			Ass.That(tests, Is.Not.Empty);
			Ass.That(tests.Count(), Is.EqualTo(3));

			var fooSuite = tests.First() as ClassSuite;
			Ass.That(fooSuite, Is.Not.Null, "First test should be a ClassSuite");
			Ass.That(fooSuite.Class, Is.EqualTo(typeof(Foo)));
			Ass.That(fooSuite.Tests.Count(), Is.EqualTo(1));

			var ignoredSuite = tests.ElementAt(1) as ClassSuite;
			Ass.That(ignoredSuite, Is.Not.Null, "Second test should be a ClassSuite");
			Ass.That(ignoredSuite.Class, Is.EqualTo(typeof(IgnoredTest)));
			Ass.That(ignoredSuite.Tests.Count(), Is.EqualTo(0));

			var testMethod = tests.Last() as ReflectedTestMethod;
			Ass.That(testMethod, Is.Not.Null, "Third test should be a ReflectedTestMethod");
			Ass.That(testMethod.Method.DeclaringType, Is.EqualTo(typeof(Bar)));
			Ass.That(testMethod.Method.Name, Is.EqualTo("BarTest"));
		}

		internal class FakeTestClass {
			[Test]
			public void Test1() { }

			[Test]
			public void Test2() { }

			public void ShouldNotBeAccumulated() { }
		}

	}
}