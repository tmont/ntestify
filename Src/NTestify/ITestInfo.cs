namespace NTestify {
	/// <summary>
	/// Encapsulates information about a test
	/// </summary>
	public interface ITestInfo {
		/// <summary>
		/// Gets or sets the description of the test
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets or sets the category for the test
		/// </summary>
		string Category { get; set; }

		/// <summary>
		/// Gets or sets the name of the test
		/// </summary>
		string Name { get; set; }
	}
}