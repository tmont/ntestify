using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NTestify {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ReflectionExtensions {
		/// <summary>
		/// A generic version of GetCustomAttributes
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribue to retrieve</typeparam>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider attributeProvider) where TAttribute : Attribute {
			return attributeProvider.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>();
		}

		/// <summary>
		/// Determines whether or not this object has an attribute
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute to look for</typeparam>
		public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider) where TAttribute : Attribute {
			return attributeProvider.GetAttributes<TAttribute>().Any();
		}

		/// <summary>
		/// Determines if the method is able to be invoked by the NTestify framework
		/// as a test method. The method must be public, non-abstract, non-static
		/// non-generic and parameterless.
		/// </summary>
		public static bool IsRunnable(this MethodInfo method) {
			return
				!method.IsConstructor && !method.IsAbstract && !method.IsGenericMethod &&
				!method.IsGenericMethodDefinition && !method.IsStatic && !method.GetParameters().Any();
		}

		/// <summary>
		/// Gets all methods for a type that testable by NTestify
		/// </summary>
		public static IEnumerable<MethodInfo> GetTestMethods(this Type type) {
			return type
				.GetMethods()
				.Where(methodInfo => methodInfo.IsTestable());
		}

		/// <summary>
		/// Gets all testable classes (types) within the assembly
		/// </summary>
		public static IEnumerable<Type> GetTestClasses(this _Assembly assembly) {
			return assembly
				.GetTypes()
				.Where(type => type.IsTestable());
		}

		/// <summary>
		/// Gets all testable methods that are not inside a testable class
		/// </summary>
		public static IEnumerable<MethodInfo> GetUnattachedTestMethods(this _Assembly assembly) {
			return assembly
				.GetTypes()
				.Where(type => !type.IsTestable())
				.SelectMany(type => type.GetTestMethods());
		}

		/// <summary>
		/// Gets the namespace + member name of the type, including generic arguments
		/// </summary>
		public static string GetFriendlyName(this Type type) {
			string name = type.Namespace + ".";
			if (type.IsGenericType) {
				//the substring crap gets rid of everything after the ` in Name, e.g. List`1 for List<T>
				name +=
					type.Name.Substring(0, type.Name.IndexOf("`")) +
					"<" +
					type
						.GetGenericArguments()
						.Select(genericType => genericType.GetFriendlyName())
						.Aggregate((current, friendlyName) => current + ", " + friendlyName) +
					">";
			} else {
				name += type.Name;
			}

			return name;
		}

		/// <summary>
		/// Determines whether a type is a numeric type (int, long, short, byte, float, double, decimal, and
		/// all the un/signed flavors)
		/// </summary>
		public static bool IsNumeric(this Type type) {
			return type.IsPrimitive && (type != typeof(object) && type != typeof(bool) && type != typeof(string) && type != typeof(char));
		}

		/// <summary>
		/// Determines whether or not a method or class is testable by virtue of being annotated
		/// with an attribute that is marked with a Testable attribute. That was quite a mouthful.
		/// </summary>
		public static bool IsTestable(this ICustomAttributeProvider attributeProvider) {
			return attributeProvider
				.GetAttributes<Attribute>()
				.Any(attribute => attribute.GetType().HasAttribute<TestableAttribute>());
		}

		/// <summary>
		/// Determines whether or not a type is able to be mocked
		/// </summary>
		/// <typeparam name="T">The type to mock</typeparam>
		public static bool IsMockable(this Type type) {
			return !type.IsSealed;
		}
	}
}