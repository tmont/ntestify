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

	/// <summary>
	/// When a method or class is annotated with a subclass, this filter will be executed after
	/// a test runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class PostTestFilter : TestifyAttribute { }

	/// <summary>
	/// When a method or class is annotated with a subclass, this filter will be executed before
	/// a test runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public abstract class PreTestFilter : TestifyAttribute { }
}