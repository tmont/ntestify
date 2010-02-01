using System;
using System.Linq;
using NTestify.Constraint;

namespace NTestify {

	/// <summary>
	/// Provides access to common assertions
	/// </summary>
	public static partial class Assert {

		/// <summary>
		/// Provides access to assertion extension methods
		/// </summary>
		public static readonly AssertionExtensionPoint Ext = new AssertionExtensionPoint();

		/// <summary>
		/// Negates and validates a constraint
		/// </summary>
		/// <typeparam name="TConstraint">The constraint to negate</typeparam>
		/// <typeparam name="TArg">The argument type of the to-be-negated constraint</typeparam>
		/// <param name="message">The message to display if the constraint is invalid</param>
		/// <param name="args">Arguments to pass to the to-be-negated constraint</param>
		public static void Not<TConstraint, TArg>(string message, params TArg[] args) where TConstraint : IConstraint where TArg : class {
			ExecuteConstraint<NotConstraint, IConstraint>(message, BuildConstraint<TConstraint, TArg>(args));
		}

		/// <summary>
		/// Uses the Activator to create a constraint object from the type parameters and given arguments
		/// </summary>
		public static TConstraint BuildConstraint<TConstraint, TArg>(params TArg[] args) where TConstraint : IConstraint where TArg : class {
			var constructor = typeof(TConstraint).GetConstructor(args.Select(o =>  o != null ? o.GetType() : typeof(Nullable) ).ToArray());
			return (TConstraint)constructor.Invoke(args);
		}

		/// <summary>
		/// Executes a constraint on a variable number of arguments
		/// </summary>
		/// <typeparam name="TConstraint">The constraint to instantiate and execute</typeparam>
		/// <typeparam name="TArg">The arguments' type</typeparam>
		/// <param name="message">The message to display if the constraint is invalid</param>
		/// <param name="args">The arguments to assert</param>
		/// <exception cref="TestAssertionException">If the constraint fails to validate</exception>
		public static void ExecuteConstraint<TConstraint, TArg>(string message, params TArg[] args) where TConstraint : IConstraint where TArg : class {
			var constraint = BuildConstraint<TConstraint, TArg>(args);
			if (!constraint.Validate()) {
				message = string.IsNullOrEmpty(message) ? string.Empty : message + "\n";
				message += constraint.FailMessage;
				throw new TestAssertionException(message);
			}
		}

	}
}