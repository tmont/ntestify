using System;
using System.Linq.Expressions;
using NTestify.Mock;
using NUnit.Framework;
using Ass = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class ExpressionEqualityTest {

		private static bool TestExpressions<T1, T2>(Expression<T1> expression1, Expression<T2> expression2) {
			return expression1.IsEqualTo(expression2);
		}

		[TestMethod]
		public void Identical_binary_expressions_should_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => x + 1,
				x => x + 1
			));
		}

		[TestMethod]
		public void Binary_expressions_with_different_operands_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => x + 1,
				x => x - 1
			), Is.False);
		}

		[TestMethod]
		public void Binary_expressions_with_different_right_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => x + 1,
				x => x + 2
			), Is.False);
		}

		[TestMethod]
		public void Binary_expressions_with_different_left_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => 1 + x,
				x => 2 + x
			), Is.False);
		}

		[TestMethod]
		public void Identical_constant_expressions_should_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => 1,
				x => 1
			));
		}

		[TestMethod]
		public void Constant_expressions_with_different_constants_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => 1,
				x => 2
			), Is.False);
		}

		[TestMethod]
		public void Identical_method_call_expressions_should_be_equal() {
			Ass.That(TestExpressions<Action<int>, Action<int>>(
				x => Console.Write(x),
				x => Console.Write(x)
			));
		}

		[TestMethod]
		public void Method_call_expressions_with_different_arguments_should_not_be_equal() {
			Ass.That(TestExpressions<Action<int>, Action<int>>(
				x => Console.Write(x),
				x => Console.Write("foo")
			), Is.False);
		}

		[TestMethod]
		public void Method_call_expressions_with_different_methods_should_not_be_equal() {
			Ass.That(TestExpressions<Action<int>, Action<int>>(
				x => Console.Write(x),
				x => Console.WriteLine(x)
			), Is.False);
		}

		[TestMethod]
		public void Identical_conditional_expressions_should_be_equal() {
			Ass.That(TestExpressions<Func<int, int, bool>, Func<int, int, bool>>(
				(x, y) => x < y,
				(x, y) => x < y
			));
		}

		[TestMethod]
		public void Conditional_expressions_with_different_operands_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int, bool>, Func<int, int, bool>>(
				(x, y) => x < y,
				(x, y) => x > y
			), Is.False);
		}

		[TestMethod]
		public void Conditional_expressions_with_different_false_results_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int, bool>, Func<int, int, bool>>(
				(x, y) => x < y ? true : false,
				// ReSharper disable ConditionIsAlwaysTrueOrFalse
				(x, y) => x > y ? true : true
				// ReSharper restore ConditionIsAlwaysTrueOrFalse
			), Is.False);
		}

		[TestMethod]
		public void Conditional_expressions_with_different_true_results_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int, bool>, Func<int, int, bool>>(
				(x, y) => x < y ? true : false,
				// ReSharper disable ConditionIsAlwaysTrueOrFalse
				(x, y) => x > y ? false : false
				// ReSharper restore ConditionIsAlwaysTrueOrFalse
			), Is.False);
		}

		[TestMethod]
		public void Expressions_with_different_parameter_names_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, int>>(
				x => x,
				y => y
			), Is.False);
		}

		[TestMethod]
		public void Expressions_with_different_types_should_not_be_equal() {
			Ass.That(TestExpressions<Func<int, int>, Func<int, float>>(
				x => x,
				y => y
			), Is.False);

			Ass.That(TestExpressions<Func<int, double>, Func<float, double>>(
				x => x,
				y => y
			), Is.False);
		}

		#region NewExpression
		[TestMethod]
		public void Identical_new_expressions_should_be_equal() {
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new object(),
				() => new object()
			));
		}

		[TestMethod]
		public void New_expressions_with_different_arguments_should_not_be_equal() {
			Ass.That(TestExpressions<Func<string>, Func<string>>(
				() => new string('x', 1),
				() => new string('x', 2)
			), Is.False);
		}

		[TestMethod]
		public void New_expressions_with_different_number_of_arguments_should_not_be_equal() {
			Ass.That(TestExpressions<Func<string>, Func<string>>(
				() => new string(new[] { 'x' }, 0, 1),
				() => new string(new[] { 'x' })
			), Is.False);
		}

		[TestMethod]
		public void New_expressions_with_different_objects_should_not_be_equal() {
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new string('x', 1),
				() => new object()
			), Is.False);
		}
		#endregion

		#region MemberInit
		[TestMethod]
		public void Identical_member_init_expressions_should_be_equal() {
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new ClassWithMembers { Foo = 1 },
				() => new ClassWithMembers { Foo = 1 }
			));
		}

		[TestMethod]
		public void Member_init_expressions_with_different_members_should_not_be_equal() {
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new ClassWithMembers { Foo = 1 },
				() => new ClassWithMembers { Bar = 1 }
			), Is.False);

			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new ClassWithMembers { Foo = 1 },
				() => new ClassWithMembers { }
			), Is.False);
		}

		[TestMethod]
		public void Member_init_expressions_with_different_member_values_should_not_be_equal() {
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => new ClassWithMembers { Foo = 1 },
				() => new ClassWithMembers { Foo = 2 }
			), Is.False);
		}
		#endregion

		[TestMethod]
		public void Identical_member_expressions_should_be_equal() {
			var foo = new ClassWithMembers();
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => foo.Foo,
				() => foo.Foo
			));
		}

		[TestMethod]
		public void Member_expressions_with_different_properties_should_not_be_equal() {
			var foo = new ClassWithMembers();
			Ass.That(TestExpressions<Func<object>, Func<object>>(
				() => foo.Foo,
				() => foo.Bar
			), Is.False);
		}

		[TestMethod]
		public void Should_ignore_parameter_names(){
			Expression<Func<int, int>> expression1 = x => x + 1;
			Expression<Func<int, int>> expression2 = y => y + 1;

			Ass.That(expression1.IsEqualTo(expression2, false));
		}

	}

	internal class ClassWithMembers {
		public int Foo { get; set; }
		public int Bar { get; set; }
	}
}