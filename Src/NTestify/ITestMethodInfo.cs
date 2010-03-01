using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Encapsulates information about a test method
	/// </summary>
	public interface ITestMethodInfo : ITestInfo {
		/// <summary>
		/// Gets the method to invoke to run this test
		/// </summary>
		MethodInfo Method { get; }
	}
}