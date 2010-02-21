using System;

namespace NTestify {

	/// <summary>
	/// When applied to another attribute, indicates that that attribute
	/// will mark whatever it's annotating as a runnable test. For example,
	/// this attribute is applied to IgnoreAttribute, TestAttribute and
	/// ExpectedExceptionAttribute within NTestify.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TestableAttribute : Attribute { }
}