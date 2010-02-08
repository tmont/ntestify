using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using NTestify.Mock.Expression;
using System.Linq.Expressions;

namespace NTestify.Mock {

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers {
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);

		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
	}

	/// <summary>
	/// A mock of a class or interface
	/// </summary>
	/// <typeparam name="T">The type to mock</typeparam>
	public class Mock<T> : IHideObjectMembers where T : class {

		private readonly IList<InvocationExpectation<T>> expectations;
		private static readonly IMockObjectBuilder Builder = new MockObjectBuilder();

		/// <summary>
		/// Initializes a new mock object
		/// </summary>
		public Mock() {
			expectations = new List<InvocationExpectation<T>>();
		}

		/// <summary>
		/// Creates an expectation for an invocation that has a return value
		/// </summary>
		/// <typeparam name="TReturn">The invocation's return value type</typeparam>
		/// <param name="invocation">The expected invocation. This can be a property getter or a method invocation that has a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T, TReturn> Expects<TReturn>(Expression<Func<T, TReturn>> invocation) {
			var expectation = new Expectation<T, TReturn>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T, TReturn>(expectation);
		}

		/// <summary>
		/// Creates an expectation for an invocation without a return value
		/// </summary>
		/// <param name="invocation">The expected invocation. This can be a property setter or a method invocation that does not have a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T> Expects(Expression<Action<T>> invocation) {
			var expectation = new Expectation<T>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T>(expectation);
		}

		/// <summary>
		/// Verifies that the expectation was met
		/// </summary>
		public void Verify() {

		}

		/// <summary>
		/// Gets the actual object instance for this mock
		/// </summary>
		public T Object { get { return Builder.Build(expectations); } }
	}

	internal class MockObjectBuilder : IMockObjectBuilder {

		private const MethodAttributes ExplicitImplementation =
			MethodAttributes.Private | MethodAttributes.HideBySig |
			MethodAttributes.NewSlot | MethodAttributes.Virtual |
			MethodAttributes.Final;

		public T Build<T>(IEnumerable<InvocationExpectation<T>> expectations) where T : class {
			if (!typeof(T).IsMockable()) {
				throw new InvalidOperationException(string.Format("The type {0} is unmockable", typeof(T).GetFriendlyName()));
			}

			var typeBuilder = CreateTypeBuilder<T>();
			DefineInvokeExpectation<T>(typeBuilder);

			throw new NotImplementedException();
		}

		private static void DefineInvokeExpectation<T>(TypeBuilder typeBuilder) where T : class {
			var methodBuilder = typeBuilder.DefineMethod("IMock.InvokeExpectation", ExplicitImplementation, null, new[] { typeof(Expectation<T>) });
			var il = methodBuilder.GetILGenerator();

		}

		private static TypeBuilder CreateTypeBuilder<T>() where T : class {
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("NTestify.Mock.Dynamic"), AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("NTestify.Mock.Dynamic");

			var parentType = typeof(T);
			var interfaces = new[] { typeof(IMock<T>) };

			return moduleBuilder.DefineType(GetMockName(typeof(T)), TypeAttributes.Class | TypeAttributes.Public, parentType, interfaces);
		}

		private static string GetMockName(Type type) {
			return "Mock_" + type.Name + "_" + Guid.NewGuid().ToString().Replace("-", "_");
		}
	}

	internal interface IMockObjectBuilder {
		T Build<T>(IEnumerable<InvocationExpectation<T>> expectations) where T : class;
	}

	internal interface IMock<T> where T : class {
		bool InvokeIfNeeded(Action<T> invocation);
		bool InvokeIfNeeded<TReturn>(Func<T, TReturn> invocation);
		IEnumerable<InvocationExpectation<T>> Expectations { get; set; }
	}

	internal class Example {
		public virtual void DoSomething(int foo) {

		}

	}

	internal class MockExample : Example, IMock<Example> {

		public override void DoSomething(int foo) {
			((IMock<Example>)this).InvokeIfNeeded(o => o.DoSomething(foo));
		}

		bool IMock<Example>.InvokeIfNeeded(Action<Example> invocation) {
			throw new NotImplementedException();
		}

		bool IMock<Example>.InvokeIfNeeded<TReturn>(Func<Example, TReturn> invocation) {
			throw new NotImplementedException();
		}

		IEnumerable<InvocationExpectation<Example>> IMock<Example>.Expectations { get; set; }
	}
}