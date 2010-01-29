using System;
using System.Linq;
using NUnit.Framework;

namespace NTestify.Tests {
	[TestFixture]
	public class TestSuiteTest {

		private object instance;
		private ExecutionContext executionContext;

		[SetUp]
		public void SetUp(){
			instance = new FakeTestSuite();
			executionContext = new ExecutionContext { Instance = instance };
		}

		[Test]
		public void Should_pass_if_suite_is_empty(){
			new TestSuite().Run(executionContext);
			Assert.That(executionContext.Result.Status, Is.EqualTo(TestStatus.Pass));
		}

	}

	internal class FakeTestSuite {

	}

}
