using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TestMethod = NUnit.Framework.TestAttribute;
using ExEx = NUnit.Framework.ExpectedExceptionAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class AssertTest {

		#region Equality Assertions
		#region Primitive types and strings
		[TestMethod]
		public void Integers_should_be_equal() {
			Assert.Equal(1, 1);
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n2 is not equal to 1.")]
		public void Integers_should_not_be_equal() {
			Assert.NotEqual(1, 2);
			Assert.Equal(1, 2);
		}

		[TestMethod]
		public void Floats_should_be_equal() {
			Assert.Equal(1.4, 1.4);
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n2.7 is not equal to 1.5.")]
		public void Floats_should_not_be_equal() {
			Assert.NotEqual(1.5, 2.7);
			Assert.Equal(1.5, 2.7);
		}

		[TestMethod]
		public void Strings_should_be_equal() {
			Assert.Equal("foo", "foo");
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected \"foo\"\nbut got  \"bar\"")]
		public void Strings_should_not_be_equal() {
			Assert.NotEqual("foo", "bar");
			Assert.Equal("foo", "bar");
		}

		[TestMethod]
		public void Chars_should_be_equal() {
			Assert.Equal('a', 'a');
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n'b' is not equal to 'a'.")]
		public void Chars_should_not_be_equal() {
			Assert.NotEqual('a', 'b');
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
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected null, got object of type System.Object.")]
		public void Null_should_not_be_equal_to_an_object() {
			Assert.NotEqual(null, new object());
			Assert.Equal(null, new object());
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected object of type System.Object, got null.")]
		public void Object_should_not_be_equal_to_null() {
			Assert.NotEqual(new object(), null);
			Assert.Equal(new object(), null);
		}

		[TestMethod]
		public void Booleans_should_be_equal() {
			Assert.Equal(true, true);
			Assert.Equal(false, false);
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nFalse is not equal to True.")]
		public void Booleans_should_not_be_equal() {
			Assert.NotEqual(true, false);
			Assert.Equal(true, false);
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Int32 is not a System.Boolean.")]
		public void Boolean_false_is_not_equal_to_integer_zero() {
			Assert.NotEqual(false, 0);
			Assert.Equal(false, 0);
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Int32 is not a System.Boolean.")]
		public void Boolean_true_is_not_equal_to_integer_one() {
			Assert.NotEqual(true, 1);
			Assert.Equal(true, 1);
		}
		#endregion

		#region Collections
		#region Size
		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 1 less item than Actual.")]
		public void Should_fail_because_expected_has_one_less_item() {
			Assert.NotEqual(new List<int> { 1, 2 }, new List<int> { 1, 2, 3 });
			Assert.Equal(new List<int> { 1, 2 }, new List<int> { 1, 2, 3 });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 1 more item than Actual.")]
		public void Should_fail_because_expected_has_one_more_item() {
			Assert.NotEqual(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 });
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 2 less items than Actual.")]
		public void Should_fail_because_expected_has_many_less_items() {
			Assert.NotEqual(new List<int> { 1 }, new List<int> { 1, 2, 3 });
			Assert.Equal(new List<int> { 1 }, new List<int> { 1, 2, 3 });
		}
		#endregion

		#region Lists
		[TestMethod]
		public void Empty_lists_with_same_type_are_equal() {
			Assert.Equal(new List<int>(), new List<int>());
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nExpected has 2 more items than Actual.")]
		public void Should_fail_because_expected_has_many_more_items() {
			Assert.NotEqual(new List<int> { 1, 2, 3 }, new List<int> { 1 });
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1 });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n4 is not equal to 3.\nActual does not contain an item in Expected at index 2: System.Int32.")]
		public void Should_fail_because_actual_does_not_contain_an_item_in_expected() {
			Assert.NotEqual(new List<int> { 1, 2, 3 }, new List<int> { 1, 2, 4 });
			Assert.Equal(new List<int> { 1, 2, 3 }, new List<int> { 1, 2, 4 });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nobject.Equals() returned false.\nActual does not contain an item in Expected at index 0: NTestify.Tests.ObjectThatIsNeverEqual.")]
		public void Should_fail_because_complex_items_in_list_are_not_equal() {
			Assert.NotEqual(new List<object> { new ObjectThatIsNeverEqual() }, new List<object> { new ObjectThatIsNeverEqual() });
			Assert.Equal(new List<object> { new ObjectThatIsNeverEqual() }, new List<object> { new ObjectThatIsNeverEqual() });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Collections.Generic.List<System.Single> is not a System.Collections.Generic.List<System.Int32>.")]
		public void Should_fail_because_of_differing_generic_types_that_are_value_types() {
			Assert.NotEqual(new List<int> { 1 }, new List<float> { 1 });
			Assert.Equal(new List<int> { 1 }, new List<float> { 1 });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Collections.Generic.List<NTestify.Tests.ObjectThatIsAlwaysEqual> is not a System.Collections.Generic.List<System.Object>.")]
		public void Should_fail_because_of_differing_generic_types_that_are_reference_types() {
			Assert.NotEqual(new List<object> { new ObjectThatIsAlwaysEqual() }, new List<ObjectThatIsAlwaysEqual> { new ObjectThatIsAlwaysEqual() });
			Assert.Equal(new List<object> { new ObjectThatIsAlwaysEqual() }, new List<ObjectThatIsAlwaysEqual> { new ObjectThatIsAlwaysEqual() });
		}

		[TestMethod]
		public void Lists_of_value_types_should_be_equal() {
			Assert.Equal(new List<int> { 1 }, new List<int> { 1 });
			Assert.Equal(new List<uint> { 1 }, new List<uint> { 1 });
			Assert.Equal(new List<byte> { 1 }, new List<byte> { 1 });
			Assert.Equal(new List<float> { 1.2f }, new List<float> { 1.2f });
			Assert.Equal(new List<double> { 1.2 }, new List<double> { 1.2 });
			Assert.Equal(new List<decimal> { 1.2m }, new List<decimal> { 1.2m });
			Assert.Equal(new List<char> { 'a' }, new List<char> { 'a' });
			Assert.Equal(new List<double> { 'a' }, new List<double> { 'a' }); //wtf?
			Assert.Equal(new List<bool> { true }, new List<bool> { true });
			Assert.Equal(new List<bool> { false }, new List<bool> { false });
		}

		[TestMethod]
		public void Lists_of_strings_should_be_equal() {
			Assert.Equal(new List<string> { "foo" }, new List<string> { "foo" });
		}

		[TestMethod]
		public void Lists_of_general_objects_should_be_equal() {
			Assert.Equal(new List<object> { new ObjectThatIsAlwaysEqual() }, new List<object> { new ObjectThatIsAlwaysEqual() });
			Assert.Equal(new List<object> { "foo" }, new List<object> { "foo" });
			Assert.Equal(new List<object> { 1 }, new List<object> { 1 });
			Assert.Equal(new List<object> { 1.2 }, new List<object> { 1.2 });
			Assert.Equal(new List<object> { 1.2f }, new List<object> { 1.2f });
			Assert.Equal(new List<object> { 1.2m }, new List<object> { 1.2m });
			Assert.Equal(new List<object> { 'a' }, new List<object> { 'a' });
			Assert.Equal(new List<object> { true }, new List<object> { true });
			Assert.Equal(new List<object> { false }, new List<object> { false });
		}

		[TestMethod]
		public void Lists_of_strongly_typed_objects_should_be_equal() {
			Assert.Equal(new List<ObjectThatIsAlwaysEqual> { new ObjectThatIsAlwaysEqual() }, new List<ObjectThatIsAlwaysEqual> { new ObjectThatIsAlwaysEqual() });
		}
		#endregion

		#region Dictionaries
		[TestMethod]
		public void Empty_dictionaries_with_same_type_are_equal() {
			Assert.Equal(new Dictionary<string, int>(), new Dictionary<string, int>());
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Collections.Generic.Dictionary<System.Int32, System.Int32> is not a System.Collections.Generic.Dictionary<System.String, System.Int32>.")]
		public void Should_fail_because_dictionary_key_type_is_different() {
			Assert.NotEqual(new Dictionary<string, int>(), new Dictionary<int, int>());
			Assert.Equal(new Dictionary<string, int>(), new Dictionary<int, int>());
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nSystem.Collections.Generic.Dictionary<System.String, System.String> is not a System.Collections.Generic.Dictionary<System.String, System.Int32>.")]
		public void Should_fail_because_dictionary_value_type_is_different() {
			Assert.NotEqual(new Dictionary<string, int>(), new Dictionary<string, string>());
			Assert.Equal(new Dictionary<string, int>(), new Dictionary<string, string>());
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nActual does not contain key foo.")]
		public void Should_fail_because_key_in_expected_does_not_exist_in_actual() {
			Assert.NotEqual(new Dictionary<string, int> { { "foo", 1 } }, new Dictionary<string, int> { { "bar", 1 } });
			Assert.Equal(new Dictionary<string, int> { { "foo", 1 } }, new Dictionary<string, int> { { "bar", 1 } });
		}

		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\n2 is not equal to 1.\nActual value does not match expected value at key foo: expected 1 but got 2.")]
		public void Should_fail_because_actual_value_is_incorrect_for_dictionaries() {
			Assert.NotEqual(new Dictionary<string, int> { { "foo", 1 } }, new Dictionary<string, int> { { "foo", 2 } });
			Assert.Equal(new Dictionary<string, int> { { "foo", 1 } }, new Dictionary<string, int> { { "foo", 2 } });
		}

		#endregion
		#endregion

		#region Objects
		[TestMethod]
		[ExEx(ExpectedException = typeof(TestAssertionException), ExpectedMessage = "Failed asserting that two objects are equal.\nNTestify.Tests.ObjectThatIsNeverEqual is not a NTestify.Tests.ObjectThatIsAlwaysEqual.")]
		public void Should_fail_because_objects_are_not_the_same_type() {
			Assert.NotEqual(new ObjectThatIsAlwaysEqual(), new ObjectThatIsNeverEqual());
			Assert.Equal(new ObjectThatIsAlwaysEqual(), new ObjectThatIsNeverEqual());
		}

		[TestMethod]
		public void Should_pass_because_objectEquals_returns_true() {
			Assert.Equal(new ObjectThatIsAlwaysEqual(), new ObjectThatIsAlwaysEqual());
		}

		[TestMethod]
		public void Should_pass_because_objects_are_the_same_reference() {
			var obj = new ObjectThatIsNeverEqual();
			Assert.Equal(obj, obj);

			var obj2 = new ObjectThatIsAlwaysEqual();
			Assert.Equal(obj2, obj2);
		}
		#endregion
		#endregion

		#region Simple Assertions
		#region Boolean Assertions
		[TestMethod]
		public void Should_be_true() {
			Assert.True(true);
		}

		[TestMethod]
		public void Should_be_false() {
			Assert.False(false);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that false is true.")]
		public void False_should_not_be_true() {
			Assert.True(false);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that true is false.")]
		public void True_should_not_be_false() {
			Assert.False(true);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an object of type System.Object is true.")]
		public void Object_should_not_be_true() {
			Assert.True(new object());
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an object of type System.Object is false.")]
		public void Object_should_not_be_false() {
			Assert.False(new object());
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that null is false.")]
		public void Null_should_not_be_false() {
			Assert.False(null);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that null is true.")]
		public void Null_should_not_be_true() {
			Assert.True(null);
		}
		#endregion

		#region Emptiness
		[TestMethod]
		public void Null_should_be_empty() {
			Assert.Empty(null);
		}

		[TestMethod]
		public void Empty_string_should_be_empty() {
			Assert.Empty(string.Empty);
		}

		[TestMethod]
		public void Empty_string_builder_should_be_empty() {
			Assert.Empty(new StringBuilder());
		}

		[TestMethod]
		public void Empty_enumerable_should_be_empty() {
			Assert.Empty(Enumerable.Empty<object>());
			Assert.Empty(Enumerable.Empty<string>());
			Assert.Empty(Enumerable.Empty<IEnumerable>());
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an object of type System.Object is empty.")]
		public void Non_enumerable_should_not_be_empty() {
			Assert.NotEmpty(new object());
			Assert.Empty(new object());
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an IEnumerable of type System.Int32[] (count=2) is empty.")]
		public void Non_empty_enumerable_should_not_be_empty() {
			var array = new[] { 1, 2 };
			Assert.NotEmpty(array);
			Assert.Empty(array);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"foo\" is empty.")]
		public void Non_empty_string_should_not_be_empty() {
			Assert.NotEmpty("foo");
			Assert.Empty("foo");
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"foo\" is empty.")]
		public void Non_empty_stringbuilder_not_be_empty() {
			var sb = new StringBuilder("foo");
			Assert.NotEmpty(sb);
			Assert.Empty(sb);
		}
		#endregion

		#region Nullness
		[TestMethod]
		public void Null_should_be_null() {
			Assert.Null(null);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an object of type System.Object is null.")]
		public void Objects_should_not_be_null() {
			Assert.NotNull(new object());
			Assert.Null(new object());
		}
		#endregion
		#endregion

		#region Collection Assertions
		#region Contains
		[TestMethod]
		public void Collection_should_contain_value() {
			Assert.Contains(new[] { 1 }, 1);
			Assert.Contains(new[] { "foo" }, "foo");
			Assert.Contains(new[] { 'a' }, 'a');
			Assert.Contains(new[] { 1m }, 1m);

			var obj = new object();
			Assert.Contains(new[] { obj }, obj);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an IEnumerable of type System.Int32[] contains 3.")]
		public void Collection_should_not_contain_value() {
			Assert.Contains(new[] { 1 }, 3);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that an IEnumerable of type System.Int32[] does not contain 1.")]
		public void Collection_contains_value_when_it_should_not() {
			Assert.NotContains(new[] { 1 }, 1);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that null contains null.")]
		public void Null_collection_never_contains_anything() {
			Assert.NotContains<object>(null, null);
			Assert.Contains<object>(null, null);
		}
		#endregion

		#region Count
		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the number of items in an object of type System.Object[] is 1.")]
		public void Null_should_be_treated_as_empty_enumerable() {
			Assert.Count<object>(null, 0); //should pass
			Assert.Count<object>(null, 1);
		}

		[TestMethod]
		public void Should_pass_because_count_is_valid() {
			Assert.Count(new[] { 1, 2, 3 }, 3);
			Assert.Count(new List<object> { new object(), new object() }, 2);
		}
		#endregion
		#endregion

		#region String Assertions
		#region Contains
		[TestMethod]
		public void String_should_contain_string() {
			Assert.Contains("foo", "f");
			Assert.Contains("foo", "o");
			Assert.Contains("foo", "fo");
			Assert.Contains("foo", "oo");
			Assert.Contains("foo", "foo");
		}

		[TestMethod]
		public void String_should_contain_null() {
			Assert.Contains("foo", null);
		}

		[TestMethod]
		public void Null_string_should_contain_null() {
			Assert.Contains(null, null);
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"\" contains the string \"foo\".")]
		public void Null_string_should_not_contain_nonempty_string() {
			const string foo = null;
			Assert.NotContains(foo, "foo");
			Assert.Contains(foo, "foo");
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"foo\" contains the string \"bar\".")]
		public void String_should_not_contain_string() {
			Assert.NotContains("foo", "bar");
			Assert.Contains("foo", "bar");
		}
		#endregion

		#region Matches
		[TestMethod]
		public void String_should_match_regex() {
			Assert.Matches("foobar", ".*");
			Assert.Matches("foobar", "^fo+");
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"foobar\" matches the regular expression \"^$\".")]
		public void String_should_not_match_regex() {
			Assert.NotMatches("foobar", "hello");
			Assert.NotMatches("foobar", "^$");
			Assert.Matches("foobar", "^$");
		}

		[TestMethod]
		[ExEx(typeof(TestAssertionException), ExpectedMessage = "Failed asserting that the string \"\" matches the regular expression \"^$\".")]
		public void Null_string_should_not_match_regex() {
			const string foo = null;
			Assert.NotMatches(foo, "hello");
			Assert.Matches(foo, "^$");
		}

		[TestMethod]
		public void Empty_string_should_match_null_regex() {
			const string foo = null;
			Assert.Matches(foo, null);
			Assert.Matches(string.Empty, null);
			Assert.Matches(foo, string.Empty);
			Assert.Matches(string.Empty, string.Empty);
		}

		[TestMethod]
		public void String_should_match_null_regex() {
			Assert.Matches("foo", null);
			Assert.Matches("foo", string.Empty);
		}
		#endregion
		#endregion

	}

	#region Mocks
	internal class ObjectThatIsAlwaysEqual {
#pragma warning disable 659
		public override bool Equals(object obj) {
#pragma warning restore 659
			return true;
		}
	}

	internal class ObjectThatIsNeverEqual {
#pragma warning disable 659
		public override bool Equals(object obj) {
#pragma warning restore 659
			return false;
		}
	}
	#endregion

}
