using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NTestify.Mock {
	internal class MockObjectBuilder {

		public T Build<T>(IEnumerable<IExpectation> expectations, object[] arguments) where T : class {
			if (!typeof(T).IsMockable()) {
				throw new InvalidOperationException(string.Format("The type {0} is unmockable", typeof(T).GetFriendlyName()));
			}

			var typeBuilder = CreateTypeBuilder<T>();

			foreach (var expectation in expectations) {
				if (expectation.GetType().IsAssignableFrom(typeof(Expectation<,>))) {
					GenerateOverride(typeBuilder, ((Expectation<T, object>)expectation));
				} else if (expectation.GetType().IsAssignableFrom(typeof(Expectation<T>))) {
					GenerateOverride(typeBuilder, ((Expectation<T>)expectation));
				}
			}

			throw new NotImplementedException();
		}

		private static void GenerateOverride<T>(TypeBuilder typeBuilder, Expectation<T> expectation) where T : class {
			var invocation = expectation.Invocation;
			
		}

		private static void GenerateOverride<T>(TypeBuilder typeBuilder, Expectation<T, object> expectation) where T : class {

		}

		private static TypeBuilder CreateTypeBuilder<T>() where T : class {
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("NTestify.Mock.Dynamic"), AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("NTestify.Mock.Dynamic");
			var parentType = typeof(T);
			return moduleBuilder.DefineType(GetMockName(typeof(T)), TypeAttributes.Class | TypeAttributes.Public, parentType);
		}

		private static string GetMockName(Type type) {
			return type.Name + "Mock_" + Guid.NewGuid().ToString().Replace("-", "_");
		}
	}

}