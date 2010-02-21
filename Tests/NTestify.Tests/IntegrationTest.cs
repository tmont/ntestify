using System;
using NUnit.Framework;
using Ass = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class IntegrationTest {

		public static string Lulz { get; set; }

		[SetUp]
		public void SetUp() {
			Lulz = string.Empty;
			MyPreFilter.Callback = () => Lulz += "prefilter ";
			MyPostFilter.Callback = () => Lulz += "postfilter ";
		}

		[TestMethod]
		public void Should_setup_and_teardown_in_the_correct_order() {
			var instance = new TestLulz();
			var test = new ReflectedTestMethod(instance.GetType().GetMethod("Test"), instance);
			test.Run(new ExecutionContext());

			const string expected = "prefilter setup1 setup2 test teardown1 teardown2 postfilter ";

			Ass.That(Lulz, Is.EqualTo(expected));
		}

		[TestMethod]
		public void Should_order_filters_correctly() {
			var instance = new TestLulz();
			var test = new ReflectedTestMethod(instance.GetType().GetMethod("Test2"), instance);
			test.Run(new ExecutionContext());

			const string expected = "setup1 setup2 prefilter test2 postfilter teardown1 teardown2 ";

			Ass.That(Lulz, Is.EqualTo(expected));
		}

		#region Mocks
		public class TestLulz {
			[Setup]
			public void Setup1() {
				Lulz += "setup1 ";
			}

			[Setup]
			public void Setup2() {
				Lulz += "setup2 ";
			}

			[TearDown]
			public void TearDown1() {
				Lulz += "teardown1 ";
			}

			[TearDown]
			public void TearDown2() {
				Lulz += "teardown2 ";
			}

			[Test, MyPreFilter, MyPostFilter]
			public void Test() {
				Lulz += "test ";
			}

			[Test, MyPreFilter(Order = FilterOrder.AfterSetup), MyPostFilter(Order = FilterOrder.BeforeTearDown)]
			public void Test2() {
				Lulz += "test2 ";
			}

			[Test(Name = "This is the name")]
			public void TestWithName(){

			}
		}

		[PreTestFilter]
		public class MyPreFilter : TestFilter {
			public static Action Callback { get; set; }

			public override void Execute(ExecutionContext executionContext) {
				Callback();
			}
		}

		[PostTestFilter]
		public class MyPostFilter : TestFilter {
			public static Action Callback { get; set; }

			public override void Execute(ExecutionContext executionContext) {
				Callback();
			}
		}
		#endregion

	}

}