using System;
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
		public static void Not<TConstraint, TArg>(string message, params TArg[] args) where TConstraint : IConstraint<TArg> {
			ExecuteConstraint<NotConstraint<TArg>, IConstraint<TArg>>(message, BuildConstraint<TConstraint, TArg>(args));
		}

		/// <summary>
		/// Uses the Activator to create a constraint object from the type parameters and given arguments
		/// </summary>
		public static TConstraint BuildConstraint<TConstraint, TArg>(params TArg[] args) where TConstraint : IConstraint<TArg> {
			return (TConstraint)Activator.CreateInstance(typeof(TConstraint), args);
		}

		/// <summary>
		/// Executes a constraint on a variable number of arguments
		/// </summary>
		/// <typeparam name="TConstraint">The constraint to instantiate and execute</typeparam>
		/// <typeparam name="TArg">The arguments' type</typeparam>
		/// <param name="message">The message to display if the constraint is invalid</param>
		/// <param name="args">The arguments to assert</param>
		/// <exception cref="TestAssertionException">If the constraint fails to validate</exception>
		public static void ExecuteConstraint<TConstraint, TArg>(string message, params TArg[] args) where TConstraint : IConstraint<TArg> {
			var constraint = BuildConstraint<TConstraint, TArg>(args);
			if (!constraint.Validate()) {
				throw new TestAssertionException(message ?? string.Empty);
			}
		}

	}
}