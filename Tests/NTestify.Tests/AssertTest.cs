using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class AssertTest {

		#region Primitive types and strings
		[TestMethod]
		public void Integers_should_be_equal() {
			Assert.Equal(1, 1);
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n2 is not equal to 1.")]
		public void Integers_should_not_be_equal() {
			Assert.Equal(1, 2);
		}

		[TestMethod]
		public void Floats_should_be_equal() {
			Assert.Equal(1.4, 1.4);
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n2.7 is not equal to 1.5.")]
		public void Floats_should_not_be_equal() {
			Assert.Equal(1.5, 2.7);
		}

		[TestMethod]
		public void Strings_should_be_equal() {
			Assert.Equal("foo", "foo");
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n\"bar\" is not equal to \"foo\".")]
		public void Strings_should_not_be_equal() {
			Assert.Equal("foo", "bar");
		}

		[TestMethod]
		public void Chars_should_be_equal() {
			Assert.Equal('a', 'a');
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nb is not equal to a.")]
		public void Chars_should_not_be_equal() {
			Assert.Equal('a', 'b');
		}

		[TestMethod]
		public void Integers_should_be_equal_to_floats() {
			Assert.Equal(1.0, 1);
			Assert.Equal(0.0, 0);
			Assert.Equal(49.0, 49);
		}

		[TestMethod]
		public void Floats_should_be_equal_to_integers() {
			Assert.Equal(1, 1.0);
			Assert.Equal(0, 0.0);
			Assert.Equal(49, 49.0);
		}

		[TestMethod]
		public void Null_should_be_equal_to_null() {
			Assert.Equal(null, null);
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected null, got object of type System.Object.")]
		public void Null_should_not_be_equal_to_an_object() {
			Assert.Equal(null, new object());
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected object of type System.Object, got null.")]
		public void An_object_should_not_be_equal_to_null() {
			Assert.Equal(new object(), null);
		}

		[TestMethod]
		public void Booleans_should_be_equal() {
			Assert.Equal(true, true);
			Assert.Equal(false, false);
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nFalse is not equal to True.")]
		public void Booleans_should_not_be_equal() {
			Assert.Equal(true, false);
		}
		#endregion

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Object is not a System.Int32.")]
		public void Should_fail_because_underlying_types_are_different() {
			Assert.Equal(1, new object());
		}

		#region Lists
		#region Size
		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 1 less item than Actual.")]
		public void Should_fail_because_expected_has_one_less_item() {
			Assert.Equal(new List<int> { 1, 2 }, new List<int> { 1, 2, 3 });
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 1 more item than Actual.")]
		public void Should_fail_because_expected_has_one_more_item() {
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 });
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 2 less items than Actual.")]
		public void Should_fail_because_expected_has_many_less_items() {
			Assert.Equal(new List<int> { 1 }, new List<int> { 1, 2, 3 });
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 2 more items than Actual.")]
		public void Should_fail_because_expected_has_many_more_items() {
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1 });
		}
		#endregion

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nActual does not contain an item in Expected at index 2: 3.")]
		public void Should_fail_because_actual_does_not_contain_an_item_in_expected() {
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1, 2, 4 });
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nActual does not contain an item in Expected at index 0: NTestify.Tests.ObjectThatIsNeverEqual.")]
		public void Should_fail_because_complex_items_are_not_equal() {
			Assert.Equal(new List<object> { new ObjectThatIsNeverEqual() }, new List<object> { new ObjectThatIsNeverEqual() });
		}

		[TestMethod]
		[ExpectedException(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Collections.Generic.List<System.Single> is not a System.Collections.Generic.List<System.Int32>.")]
		public void Should_fail_because_of_differing_generic_types() {
			Assert.Equal(new List<int> { 1 }, new List<float> { 1 });
		}
		#endregion

	}

	internal class ObjectThatIsAlwaysEqual {
		public override bool Equals(object obj) {
			return true;
		}
	}

	internal class ObjectThatIsNeverEqual {
		public override bool Equals(object obj) {
			return false;
		}
	}

}
