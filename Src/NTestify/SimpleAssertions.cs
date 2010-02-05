using NTestify.Constraint;

namespace NTestify {
	public static partial class Assert {
		/// <summary>
		/// Asserts that a value is true
		/// </summary>
		/// <param name="valueThatShouldBeTrue">Value that should be true</param>
		public static void True(object valueThatShouldBeTrue) {
			ExecuteConstraint(new BooleanConstraint(true, valueThatShouldBeTrue), null);
		}

		/// <summary>
		/// Asserts that a value is true, displaying the specified message upon failure
		/// </summary>
		/// <param name="valueThatShouldBeTrue">Value that should be true</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void True(object valueThatShouldBeTrue, string message) {
			ExecuteConstraint(new BooleanConstraint(true, valueThatShouldBeTrue), message);
		}

		/// <summary>
		/// Asserts that a value is false
		/// </summary>
		/// <param name="valueThatShouldBeFalse">Value that should be false</param>
		public static void False(object valueThatShouldBeFalse) {
			ExecuteConstraint(new BooleanConstraint(false, valueThatShouldBeFalse), null);
		}

		/// <summary>
		/// Asserts that a value is false, displaying the specified message upon failure
		/// </summary>
		/// <param name="valueThatShouldBeFalse">Value that should be false</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void False(object valueThatShouldBeFalse, string message) {
			ExecuteConstraint(new BooleanConstraint(false, valueThatShouldBeFalse), message);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is empty
		/// </summary>
		/// <param name="emptyObject">The object that should be empty</param>
		public static void Empty(object emptyObject) {
			ExecuteConstraint(new EmptyConstraint(emptyObject), null);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is empty, displaying the specified message upon failure
		/// </summary>
		/// <param name="emptyObject">The object that should be empty</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void Empty(object emptyObject, string message) {
			ExecuteConstraint(new EmptyConstraint(emptyObject), message);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is not empty
		/// </summary>
		/// <param name="objectThatIsNotEmpty">The object that should not be empty</param>
		public static void NotEmpty(object objectThatIsNotEmpty) {
			Not(new EmptyConstraint(objectThatIsNotEmpty), null);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is not empty, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatIsNotEmpty">The object that should not be empty</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void NotEmpty(object objectThatIsNotEmpty, string message) {
			Not(new EmptyConstraint(objectThatIsNotEmpty), message);
		}

		/// <summary>
		/// Asserts that an object is null
		/// </summary>
		/// <param name="objectThatShouldBeNull">The object that should be null</param>
		public static void Null(object objectThatShouldBeNull) {
			ExecuteConstraint(new NullConstraint(objectThatShouldBeNull), null);
		}

		/// <summary>
		/// Asserts that an object is null, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatShouldBeNull">The object that should be null</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void Null(object objectThatShouldBeNull, string message) {
			ExecuteConstraint(new NullConstraint(objectThatShouldBeNull), message);
		}

		/// <summary>
		/// Asserts that an object is not null
		/// </summary>
		/// <param name="objectThatShouldNotBeNull">The object that should not be null</param>
		public static void NotNull(object objectThatShouldNotBeNull) {
			Not(new NullConstraint(objectThatShouldNotBeNull), null);
		}

		/// <summary>
		/// Asserts that an object is not null, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatShouldNotBeNull">The object that should not be null</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void NotNull(object objectThatShouldNotBeNull, string message) {
			Not(new NullConstraint(objectThatShouldNotBeNull), message);
		}
	}
}