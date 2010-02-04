using NTestify.Constraint;

namespace NTestify {
	public static partial class Assert {
		/// <summary>
		/// Asserts that a value is true
		/// </summary>
		/// <param name="valueThatShouldBeTrue">Value that should be true</param>
		public static void True(object valueThatShouldBeTrue) {
			ExecuteConstraint<BooleanConstraint, object>(null, true, valueThatShouldBeTrue);
		}

		/// <summary>
		/// Asserts that a value is true, displaying the specified message upon failure
		/// </summary>
		/// <param name="valueThatShouldBeTrue">Value that should be true</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void True(object valueThatShouldBeTrue, string message) {
			ExecuteConstraint<BooleanConstraint, object>(message, true, valueThatShouldBeTrue);
		}

		/// <summary>
		/// Asserts that a value is false
		/// </summary>
		/// <param name="valueThatShouldBeFalse">Value that should be false</param>
		public static void False(object valueThatShouldBeFalse) {
			ExecuteConstraint<BooleanConstraint, object>(null, false, valueThatShouldBeFalse);
		}

		/// <summary>
		/// Asserts that a value is false, displaying the specified message upon failure
		/// </summary>
		/// <param name="valueThatShouldBeFalse">Value that should be false</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void False(object valueThatShouldBeFalse, string message) {
			ExecuteConstraint<BooleanConstraint, object>(message, false, valueThatShouldBeFalse);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is empty
		/// </summary>
		/// <param name="emptyObject">The object that should be empty</param>
		public static void Empty(object emptyObject) {
			ExecuteConstraint<EmptyConstraint, object>(null, emptyObject);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is empty, displaying the specified message upon failure
		/// </summary>
		/// <param name="emptyObject">The object that should be empty</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void Empty(object emptyObject, string message) {
			ExecuteConstraint<EmptyConstraint, object>(message, emptyObject);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is not empty
		/// </summary>
		/// <param name="objectThatIsNotEmpty">The object that should not be empty</param>
		public static void NotEmpty(object objectThatIsNotEmpty) {
			ExecuteConstraint<EmptyConstraint, object>(null, objectThatIsNotEmpty);
		}

		/// <summary>
		/// Asserts that an object/array/enumerable/string is not empty, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatIsNotEmpty">The object that should not be empty</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void NotEmpty(object objectThatIsNotEmpty, string message) {
			ExecuteConstraint<EmptyConstraint, object>(message, objectThatIsNotEmpty);
		}

		/// <summary>
		/// Asserts that an object is null
		/// </summary>
		/// <param name="objectThatShouldBeNull">The object that should be null</param>
		public static void Null(object objectThatShouldBeNull) {
			ExecuteConstraint<NullConstraint, object>(null, objectThatShouldBeNull);
		}

		/// <summary>
		/// Asserts that an object is null, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatShouldBeNull">The object that should be null</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void Null(object objectThatShouldBeNull, string message) {
			ExecuteConstraint<NullConstraint, object>(message, objectThatShouldBeNull);
		}

		/// <summary>
		/// Asserts that an object is not null
		/// </summary>
		/// <param name="objectThatShouldNotBeNull">The object that should not be null</param>
		public static void NotNull(object objectThatShouldNotBeNull) {
			ExecuteConstraint<NullConstraint, object>(null, objectThatShouldNotBeNull);
		}

		/// <summary>
		/// Asserts that an object is not null, displaying the specified message upon failure
		/// </summary>
		/// <param name="objectThatShouldNotBeNull">The object that should not be null</param>
		/// <param name="message">The message to display if the assertion fails</param>
		public static void NotNull(object objectThatShouldNotBeNull, string message) {
			ExecuteConstraint<NullConstraint, object>(message, objectThatShouldNotBeNull);
		}
	}
}