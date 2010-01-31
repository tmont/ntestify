using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NTestify {
	public static class ReflectionExtensions {
		/// <summary>
		/// Gets whether or not this member should be ignored by NTestify
		/// </summary>
		public static bool ShouldBeIgnored(this MemberInfo member) {
			return member.GetAttributes<IgnoreAttribute>().Any();
		}

		/// <summary>
		/// Gets the reason the test was ignored
		/// </summary>
		public static string GetIgnoreReason(this MemberInfo member) {
			return member.GetAttributes<IgnoreAttribute>().First().Reason;
		}

		/// <summary>
		/// A generic version of GetCustomAttributes
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribue to retrieve</typeparam>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo member) where TAttribute : Attribute {
			return member.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>();
		}

		/// <summary>
		/// Determines whether or not a member has an attribute
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute to look for</typeparam>
		public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute {
			return member.GetAttributes<TAttribute>().Any();
		}

		/// <summary>
		/// Determines if the method is able to be invoked by the NTestify framework
		/// as a test method
		/// </summary>
		public static bool IsRunnable(this MethodInfo method) {
			return
				!method.IsConstructor && !method.IsAbstract &&
				!method.IsStatic && !method.GetParameters().Any();
		}

		/// <summary>
		/// Gets all methods for a type that testable by NTestify
		/// </summary>
		public static IEnumerable<MethodInfo> GetTestMethods(this Type type) {
			return type
				.GetMethods()
				.Where(methodInfo => methodInfo.HasAttribute<TestableAttribute>());
		}

		/// <summary>
		/// Gets all testable classes (types) within the assembly
		/// </summary>
		public static IEnumerable<Type> GetTestClasses(this _Assembly assembly) {
			return assembly
				.GetTypes()
				.Where(type => type.HasAttribute<TestableAttribute>());
		}

		/// <summary>
		/// Gets all testable methods that are not inside a testable class
		/// </summary>
		public static IEnumerable<MethodInfo> GetUnattachedTestMethods(this _Assembly assembly) {
			return assembly
				.GetTypes()
				.Where(type => !type.HasAttribute<TestableAttribute>())
				.SelectMany(type => type.GetTestMethods());
		}
	}
}