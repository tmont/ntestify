using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using NTestify.Mock;

namespace NTestify.Tests {
	[TestFixture]
	public class BuilderTest {

		[NUnit.Framework.Test, NUnit.Framework.Ignore]
		public void ILTest() {
			var assemblyName = new AssemblyName("NTestify.Mock.Dynamic");
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

			var parentType = typeof(Example);
			//var interfaces = new[] { typeof(IMock<Example>) };
			

			var typeBuilder = moduleBuilder.DefineType("MyLulz", TypeAttributes.Class | TypeAttributes.Public, parentType/*, interfaces*/);
			typeBuilder.DefineField("lollers", typeof(int), FieldAttributes.Static);
			typeBuilder.CreateType();

			assemblyBuilder.Save("test2.dll");
		}
	}

	public class Example {
		public Example() { }
		public Example(string foo) { }
		public virtual void DoSomething(int foo) { }
		public virtual int DoSomething() { return default(int); }
		public void SimplestAction(){
			Action<int> x = y => { };
		}

		public Func<int> SimplestFunc() {
			return () => 1;
		}
	}

	public class MockExample : Example {
		public MockExample(string foo) : base(foo) { }

		public override void DoSomething(int foo) {
			ExpectationRegistry.Invoke<Example>(o => o.DoSomething(foo));
		}

		public override int DoSomething() {
			return ExpectationRegistry.Invoke<Example, int>(o => o.DoSomething());
		}

	}
}