using System;
using System.Reflection;

namespace NTestify {
	/// <summary>
	/// A test filter that invokes a method upon execution
	/// </summary>
	public abstract class InvokableFilter : TestFilter {
		public sealed override void Execute(ExecutionContext executionContext) {
			if (Method == null) {
				throw new InvalidOperationException("A method must be specified");
			}

			Method.Invoke(executionContext.Instance, new object[0]);
		}

		/// <summary>
		/// The method to invoke
		/// </summary>
		protected internal MethodInfo Method { get; set; }
	}
}