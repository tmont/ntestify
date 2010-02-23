using System.Reflection;

namespace NTestify {
	/// <summary>
	/// Encapsulates information about a test
	/// </summary>
	public interface ITestInfo {
		/// <summary>
		/// Description of the test
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Name of the category for this test
		/// </summary>
		string Category { get; set; }

		/// <summary>
		/// Name of the test
		/// </summary>
		string Name { get; set; }
	}

	/// <summary>
	/// Encapsulates information about a test method
	/// </summary>
	public interface ITestMethodInfo : ITestInfo {
		/// <summary>
		/// The method to invoke to run this test
		/// </summary>
		MethodInfo Method { get; }
	}
}