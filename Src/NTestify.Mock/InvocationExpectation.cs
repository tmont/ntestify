using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTestify.Mock {
	/// <summary>
	/// Represents an expectation. For example, "a method is expected
	/// to be invoked once and return foo."
	/// </summary>
	/// <typeparam name="T">The type this invocation expects to operate on</typeparam>
	/// <typeparam name="TInvocation">The type of delegate the type invokes</typeparam>
	public abstract class InvocationExpectation<T, TInvocation> : IExpectation
		where T : class
		where TInvocation : LambdaExpression {
		private int expectedInvocationCount;
		private readonly TInvocation invocation;

		protected InvocationExpectation(TInvocation invocation) {
			ExpectedInvocationCount = 1;
			this.invocation = invocation;
			ParseInvocation(invocation.Body);
		}

		private void ParseMemberExpression(MemberExpression memberExpression) {
			var caller = memberExpression.Expression as ParameterExpression;
			if (caller == null || caller.Type != typeof(T)) {
				throw new InvalidExpectationException(string.Format("Invocations on properties must operate on {0}", typeof(T).GetFriendlyName()));
			}

			MethodOrPropertyName = memberExpression.Member.Name;
			Arguments = Enumerable.Empty<object>();
		}

		private void ParseMethodCallExpression(MethodCallExpression methodCall) {
			if (!methodCall.VerifyCaller<T>()) {
				throw new InvalidExpectationException(string.Format("Invocation must make a call to a method or invoke a property on {0}", typeof(T).GetFriendlyName()));
			}

			MethodOrPropertyName = methodCall.Method.Name;

			var arguments = new List<object>();
			foreach (var argExpression in methodCall.Arguments) {
				if (argExpression is MethodCallExpression) {
					var argCallExpression = (MethodCallExpression)argExpression;
					if (argCallExpression.Object == null && argCallExpression.Method.DeclaringType == typeof(Arg)) {
						if (argCallExpression.Method.Name == "Any") {
							arguments.Add(new AnyArg(argCallExpression.Method.GetGenericArguments()[0]));
							continue;
						}

						//just in case
						throw new NotImplementedException();
					}
				}

				//just add the literal value
				arguments.Add(Expression.Lambda(argExpression).Compile().DynamicInvoke());
			}

			Arguments = arguments;
		}

		private void ParseInvocation(Expression body) {
			if (body is MemberExpression) {
				ParseMemberExpression((MemberExpression)body);
			} else if (body is MethodCallExpression) {
				ParseMethodCallExpression((MethodCallExpression)body);
			} else {
				throw new InvalidExpectationException("Invocation must be a MemberExpression or a MethodCallExpression");
			}
		}		

		/// <summary>
		/// Gets the name of the property or method that will be invoked
		/// </summary>
		public string MethodOrPropertyName { get; private set; }

		/// <summary>
		/// Gets the arguments (if relevant) for the invocation
		/// </summary>
		public IEnumerable<object> Arguments { get; private set; }

		/// <summary>
		/// Gets or sets the number of times this invocation is expected to be invoked. The default value
		/// is one, so if this expectation is never expected to be invoked, explicitly set
		/// this value to zero.
		/// </summary>
		/// <value>The number of times this invocation is expected to be invoked; must be greater than or equal to zero</value>
		public int ExpectedInvocationCount {
			get { return expectedInvocationCount; }
			set {
				if (value < 0) {
					throw new ArgumentException("Invocation count must be greater than or equal to zero", "value");
				}

				expectedInvocationCount = value;
			}
		}

		/// <summary>
		/// Gets the number of times this expectation has been invoked
		/// </summary>
		public int ActualInvocationCount { get; protected set; }

		/// <summary>
		/// The callback that is expected to be executed when the expectation
		/// is invoked
		/// </summary>
		public Action<T> Callback { get; set; }

		/// <summary>
		/// The invocation for this expectation
		/// </summary>
		public TInvocation Invocation { get { return invocation; } }

		/// <summary>
		/// Creates and formats an exception to be raised when the number of actual
		/// invocations exceeds the expected
		/// </summary>
		protected MockInvocationException CreateInvocationException(int expected, int actual) {
			return new MockInvocationException(
				string.Format(
					"Invocation {0} was invoked {1} {2}, but only expected to be invoked {3} {4}",
					ToString(),
					expected,
					(expected == 1) ? "time" : "times",
					actual,
					(actual == 1) ? "time" : "times"
					)
				);
		}

		/// <summary>
		/// Determines whether the given expression matches the invocation of
		/// this expectation
		/// </summary>
		public bool Matches(LambdaExpression expression) {
			if (expression.Parameters.Count != 1) {
				return false;
			}

			if (expression.Parameters[0].Type != typeof(T)) {
				return false;
			}

			if (expression.Body is MethodCallExpression) {
				return MatchMethodCall((MethodCallExpression)expression.Body);
			} else if (expression.Body is MemberExpression) {
				return MatchMemberCall((MemberExpression)expression.Body);
			}

			return false;
		}

		private bool MatchMemberCall(MemberExpression member){
			if (member.Member.Name != MethodOrPropertyName) {
				return false;
			}

			var caller = member.Expression as ParameterExpression;
			if (caller == null) {
				return false;
			}

			if (caller.Type != typeof(T)) {
				return false;
			}

			return true;
		}

		private bool MatchMethodCall(MethodCallExpression methodCall){
			if (!methodCall.VerifyCaller<T>()) {
				return false;
			}

			if (methodCall.Method.Name != MethodOrPropertyName) {
				return false;
			}

			if (methodCall.Arguments.Count != Arguments.Count()) {
				return false;
			}

			var args = methodCall
				.Arguments
				.Select(argExpression => Expression.Lambda(argExpression).Compile().DynamicInvoke());

			for (var i = 0; i < Arguments.Count(); i++) {
				var expectedArg = Arguments.ElementAt(i);
				var actualArg = args.ElementAt(i);
				if (expectedArg is ISpecialArgument) {
					if (!((ISpecialArgument)expectedArg).Matches(actualArg)) {
						return false;
					}
				} else {
					if (actualArg != null) {
						if (!actualArg.Equals(expectedArg)) {
							return false;
						}
					} else if (expectedArg != null) {
						return false;
					}
				}
			}

			return true;
		}

		public override string ToString() {
			return Invocation.ToString();
		}
		}
}