using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTestify {
	public static class ReflectionExtensions {
		/// <summary>
		/// Gets whether or not this member should be ignored by NTestify
		/// </summary>
		public static bool ShouldBeIgnored(this MemberInfo member){
			return member.GetAttributes<IgnoreAttribute>().Any();
		}

		/// <summary>
		/// Gets the reason the test was ignored
		/// </summary>
		public static string GetIgnoreReason(this MemberInfo member){
			return member.GetAttributes<IgnoreAttribute>().First().Reason;
		}

		/// <summary>
		/// A generic version of GetCustomAttributes
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribue to retrieve</typeparam>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo member) where TAttribute : Attribute {
			return member.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>();
		}
	}
}