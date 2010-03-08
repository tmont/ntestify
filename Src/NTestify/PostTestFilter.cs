using System;

namespace NTestify {
	/// <summary>
	/// When a TestifyAttribute subclass is annotated with this attribute, it signifies that
	/// it is a filter that will be executed before a test runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class PostTestFilter : Attribute { }

	/// <summary>
	/// When a TestifyAttribute subclass is annotated with this attribute, it signifies that
	/// it is a filter that will be executed after a suite runs
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class PostSuiteFilter : Attribute { }
}