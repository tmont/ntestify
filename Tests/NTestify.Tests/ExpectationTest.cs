using System;
using System.Linq;
using System.Linq.Expressions;
using NTestify.Mock;
using NUnit.Framework;
using Ass = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NTestify.Tests {
	[TestFixture]
	public class ExpectationTest {

		[TestMethod]
		public void Should_parse_parameterless_void_invocation() {
			var expectation = new Expectation<ClassToMock>(x => x.DoStuff());

			Ass.That(expectation.MethodOrPropertyName, Is.EqualTo("DoStuff"));
			Ass.That(expectation.Arguments, Is.Empty);
		}

		[TestMethod]
		public void Should_parse_void_invocation_with_verbatim_parameters(){
			var expectation = new Expectation<ClassToMock>(x => x.DoStuff(1, "bar"));

			Ass.That(expectation.MethodOrPropertyName, Is.EqualTo("DoStuff"));
			Ass.That(expectation.Arguments.Count(), Is.EqualTo(2));
			Ass.That(expectation.Arguments.First(), Is.EqualTo(1));
			Ass.That(expectation.Arguments.Last(), Is.EqualTo("bar"));
		}

		[TestMethod]
		public void Should_parse_void_invocation_with_special_parameters() {
			var expectation = new Expectation<ClassToMock>(x => x.DoStuff(Arg.Any<int>(), "bar"));

			Ass.That(expectation.MethodOrPropertyName, Is.EqualTo("DoStuff"));
			Ass.That(expectation.Arguments.Count(), Is.EqualTo(2));

			var any = expectation.Arguments.First() as AnyArg;
			Ass.That(any, Is.Not.Null);
			Ass.That(any.Matches(0));
			Ass.That(any.Matches(int.MaxValue));
			Ass.That(any.Matches(int.MinValue));

			Ass.That(expectation.Arguments.Last(), Is.EqualTo("bar"));
		}

		[TestMethod]
		public void Should_match_simple_expression(){
			const int foo = 3;
			var expectation1 = new Expectation<ClassToMock, int>(x => x.ReturnSomething(3));
			var expectation2 = new Expectation<ClassToMock, int>(x => x.ReturnSomething(foo));

			Expression<Func<ClassToMock, int>> expression1 = x => x.ReturnSomething(3);
			Expression<Func<ClassToMock, int>> expression2 = x => x.ReturnSomething(foo);

			Ass.That(expectation1.Matches(expression1));
			Ass.That(expectation1.Matches(expression2));
			Ass.That(expectation2.Matches(expression1));
			Ass.That(expectation2.Matches(expression2));
		}

		[TestMethod]
		public void Should_match_complex_expression() {
			var expectation = new Expectation<ClassToMock, int>(x => x.ReturnSomething(Arg.Any<int>()));

			Expression<Func<ClassToMock, int>> expression1 = x => x.ReturnSomething(3);
			Expression<Func<ClassToMock, int>> expression2 = x => x.ReturnSomething(default(int));
			Expression<Func<ClassToMock, int>> expression3 = x => x.ReturnSomething(int.MinValue);

			Ass.That(expectation.Matches(expression1));
			Ass.That(expectation.Matches(expression2));
			Ass.That(expectation.Matches(expression3));
		}

		private class ClassToMock {
			public int Count { get; set; }

			public void DoStuff() { }

			public void DoStuff(int foo, string bar) {

			}

			public int ReturnSomething(int foo){
				return foo;
			}

		}
	}
}