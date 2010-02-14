using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace NTestify.Mock {

	public static class ExpressionExtensions {
		/// <summary>
		/// Gets the calling object instance of a method call expression
		/// </summary>
		/// <typeparam name="T">The type of the calling object</typeparam>
		public static bool VerifyCaller<T>(this MethodCallExpression methodCall) where T : class {
			if (methodCall.Object is ParameterExpression) {
				if (methodCall.Object.Type != typeof(T)) {
					return false;
				}
			} else {
				throw new NotImplementedException(string.Format("I don't know how to handle a {0}", methodCall.Object.Type.GetFriendlyName()));
			}

			return true;
		}

	}

	public static class ExpressionEqualityExtensions {

		public static bool IsEqualTo(this Expression original, Expression other, bool parameterNamesMatter) {
			if (other == null || original.NodeType != other.NodeType || original.Type != other.Type) {
				return false;
			}

			if (original is LambdaExpression) {
				return LambdaExpressionsAreEqual((LambdaExpression)original, (LambdaExpression)other, parameterNamesMatter);
			} else if (original is BinaryExpression) {
				return BinaryExpressionsAreEqual((BinaryExpression)original, (BinaryExpression)other, parameterNamesMatter);
			} else if (original is ParameterExpression) {
				return ParameterExpressionsAreEqual((ParameterExpression)original, (ParameterExpression)other, parameterNamesMatter);
			} else if (original is UnaryExpression) {
				return UnaryExpressionsAreEqual((UnaryExpression)original, (UnaryExpression)other);
			} else if (original is ConstantExpression) {
				return ConstantExpressionsAreEqual((ConstantExpression)original, (ConstantExpression)other);
			} else if (original is MethodCallExpression) {
				return MethodCallExpressionsAreEqual((MethodCallExpression)original, (MethodCallExpression)other);
			} else if (original is MemberExpression) {
				return MemberExpressionsAreEqual((MemberExpression)original, (MemberExpression)other);
			} else if (original is MemberInitExpression) {
				return MemberInitExpressionsAreEqual((MemberInitExpression)original, (MemberInitExpression)other);
			} else if (original is ConditionalExpression) {
				return ConditionalExpressionsAreEqual((ConditionalExpression)original, (ConditionalExpression)other);
			} else if (original is NewExpression) {
				return NewExpressionsAreEqual((NewExpression)original, (NewExpression)other);
			}

			throw new NotImplementedException("That expression type is unhandleable");
		}

		private static bool ParameterExpressionsAreEqual(ParameterExpression expression1, ParameterExpression expression2, bool parameterNamesMatter) {
			return
				(!parameterNamesMatter || expression1.Name == expression2.Name) &&
				expression1.NodeType == expression2.NodeType &&
				expression1.Type == expression2.Type;
		}

		public static bool IsEqualTo(this System.Linq.Expressions.Expression original, System.Linq.Expressions.Expression other) {
			return original.IsEqualTo(other, true);
		}

		private static bool ConditionalExpressionsAreEqual(ConditionalExpression expression1, ConditionalExpression expression2) {
			if (!expression1.IfFalse.IsEqualTo(expression2.IfFalse)) {
				return false;
			}
			if (!expression1.IfTrue.IsEqualTo(expression2.IfTrue)) {
				return false;
			}
			if (!expression1.Test.IsEqualTo(expression2.Test)) {
				return false;
			}

			return true;
		}

		private static bool MemberInitExpressionsAreEqual(MemberInitExpression expression1, MemberInitExpression expression2) {
			if (expression1.Bindings.Count != expression2.Bindings.Count) {
				return false;
			}

			for (var i = 0; i < expression1.Bindings.Count; i++) {
				if (!BindingsAreEqual(expression1.Bindings[i], expression2.Bindings[i])) {
					return false;
				}
			}

			if (!NewExpressionsAreEqual(expression1.NewExpression, expression2.NewExpression)) {
				return false;
			}

			return true;
		}

		private static bool BindingsAreEqual(MemberBinding binding1, MemberBinding binding2) {
			if (binding1.BindingType != binding2.BindingType) {
				return false;
			}

			if (binding1.Member != binding2.Member) {
				return false;
			}

			if (binding1 is MemberAssignment) {
				if (!(binding2 is MemberAssignment)) {
					return false;
				}

				var memberAssignment1 = binding1 as MemberAssignment;
				var memberAssignment2 = binding2 as MemberAssignment;

				if (memberAssignment1.Expression != null) {
					if (!memberAssignment1.Expression.IsEqualTo(memberAssignment2.Expression)) {
						return false;
					}
				} else if (memberAssignment2.Expression != null) {
					return false;
				}
			}

			return true;
		}

		private static bool NewExpressionsAreEqual(NewExpression expression1, NewExpression expression2) {
			if (expression1.Constructor != expression2.Constructor) {
				return false;
			}

			if (expression1.Arguments.Count != expression2.Arguments.Count) {
				return false;
			}

			for (var i = 0; i < expression1.Arguments.Count; i++) {
				if (!expression1.Arguments[i].IsEqualTo(expression2.Arguments[i])) {
					return false;
				}
			}

			if (expression1.Members == null && expression2.Members == null) {
				return true;
			}
			if (!NullCheck(expression1.Members, expression2.Members)) {
				return false;
			}

			if (expression1.Members.Count != expression2.Members.Count) {
				return false;
			}

			for (var i = 0; i < expression1.Members.Count; i++) {
				if (!expression1.Members[i].Equals(expression2.Members[i])) {
					return false;
				}
			}

			return true;
		}

		private static bool MemberExpressionsAreEqual(MemberExpression expression1, MemberExpression expression2) {
			if (expression1.Member != expression2.Member) {
				return false;
			}

			return expression1.Expression.IsEqualTo(expression2.Expression);
		}

		private static bool MethodCallExpressionsAreEqual(MethodCallExpression expression1, MethodCallExpression expression2) {
			if (expression1.Method != expression2.Method) {
				return false;
			}

			if (expression1.Arguments.Count != expression2.Arguments.Count) {
				return false;
			}

			for (var i = 0; i < expression1.Arguments.Count; i++) {
				if (!expression1.Arguments[i].IsEqualTo(expression2.Arguments[i])) {
					return false;
				}
			}

			if (expression1.Object == null && expression2.Object == null) {
				return true;
			}
			if (!NullCheck(expression1.Object, expression2.Object)) {
				return false;
			}
			if (!expression1.Object.Equals(expression2.Object)) {
				return false;
			}

			return true;
		}

		private static bool ConstantExpressionsAreEqual(ConstantExpression expression1, ConstantExpression expression2) {
			if (!NullCheck(expression1, expression2)) {
				return false;
			}
			if (expression1 == null) {
				return true; //both null
			}

			return expression1.Value.Equals(expression2.Value);
		}

		private static bool UnaryExpressionsAreEqual(UnaryExpression expression1, UnaryExpression expression2) {
			if (expression1.IsLifted != expression2.IsLifted) {
				return false;
			}

			if (expression1.IsLiftedToNull != expression2.IsLiftedToNull) {
				return false;
			}

			if (expression1.Method != expression2.Method) {
				return false;
			}

			return expression1.Operand.IsEqualTo(expression2.Operand);
		}

		private static bool NullCheck(object thing1, object thing2) {
			if (thing1 == null) {
				if (thing2 == null) {
					return true;
				}

				return false;
			}

			if (thing2 == null) {
				return false;
			}

			return true;
		}

		private static bool LambdaExpressionsAreEqual(LambdaExpression expression1, LambdaExpression expression2, bool parameterNamesMatter) {
			if (!NullCheck(expression1, expression2)) {
				return false;
			}
			if (expression1 == null) {
				return true;
			}

			if (!ParametersAreEqual(expression1.Parameters, expression2.Parameters, parameterNamesMatter)) {
				return false;
			}

			return expression1.Body.IsEqualTo(expression2.Body, parameterNamesMatter);
		}

		private static bool BinaryExpressionsAreEqual(BinaryExpression expression1, BinaryExpression expression2, bool parameterNamesMatter) {
			if (expression1.IsLifted != expression2.IsLifted) {
				return false;
			}

			if (expression1.IsLiftedToNull != expression2.IsLiftedToNull) {
				return false;
			}

			if (expression1.Method != expression2.Method) {
				return false;
			}

			if (!LambdaExpressionsAreEqual(expression1.Conversion, expression2.Conversion, parameterNamesMatter)) {
				return false;
			}

			return expression1.Left.IsEqualTo(expression2.Left, parameterNamesMatter) && expression1.Right.IsEqualTo(expression2.Right, parameterNamesMatter);
		}

		private static bool ParametersAreEqual(ReadOnlyCollection<ParameterExpression> params1, ReadOnlyCollection<ParameterExpression> params2, bool parameterNamesMatter) {
			if (params1.Count != params2.Count) {
				return false;
			}

			for (var i = 0; i < params1.Count; i++) {
				if (!params1[i].IsEqualTo(params2[i], parameterNamesMatter)) {
					return false;
				}
			}

			return true;
		}
	}
}