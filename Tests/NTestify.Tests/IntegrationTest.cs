using NUnit.Framework;
using Ass = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class IntegrationTest {

		[TestMethod]
		public void Should_setup_and_teardown() {
			var instance = new TestLulz();
			var test = new ReflectedTestMethod(instance.GetType().GetMethod("Test"), instance);
			test.Run(new ExecutionContext());

			Ass.That(TestLulz.SetupCalled);
			Ass.That(TestLulz.TearDownCalled);
		}


		public class TestLulz {

			public static bool SetupCalled { get; private set; }
			public static bool TearDownCalled { get; private set; }

			[Setup]
			public void Setup() {
				SetupCalled = true;
			}

			[TearDown]
			public void TearDown(){
				TearDownCalled = true;
			}

			[Test]
			public void Test(){

			}
		}

	}

}