namespace NTestify.Execution {
	public class NamespaceAccumulationFilter : IAccumulationFilter {

		public string Namespace { get; set; }

		public bool Filter(ITest test){
			return !(test is ReflectedTestMethod) || ((ReflectedTestMethod)test).Method.DeclaringType.Namespace == Namespace;
		}
	}
}