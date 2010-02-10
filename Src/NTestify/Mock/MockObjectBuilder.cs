using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NTestify.Mock {
	internal interface IMockObjectBuilder {
		T Build<T>(IEnumerable<IExpectation> expectations, object[] arguments) where T : class;
	}

	internal class MockObjectBuilder : IMockObjectBuilder {

		private const MethodAttributes ExplicitImplementation =
			MethodAttributes.Private | MethodAttributes.HideBySig |
			MethodAttributes.NewSlot | MethodAttributes.Virtual |
			MethodAttributes.Final;

		public T Build<T>(IEnumerable<IExpectation> expectations, object[] arguments) where T : class {
			if (!typeof(T).IsMockable()) {
				throw new InvalidOperationException(string.Format("The type {0} is unmockable", typeof(T).GetFriendlyName()));
			}

			var typeBuilder = CreateTypeBuilder<T>();
			DefineInvokeExpectation<T>(typeBuilder);

			throw new NotImplementedException();
		}

		private static void DefineInvokeExpectation<T>(TypeBuilder typeBuilder) where T : class {
			var methodBuilder = typeBuilder.DefineMethod("IMock.InvokeExpectation", ExplicitImplementation, null, new[] { typeof(Expectation<T>) });
			var  il = methodBuilder.GetILGenerator();

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
}