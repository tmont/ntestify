namespace NTestify {
	/// <summary>
	/// Base class for test filters
	/// </summary>
	public abstract class TestFilter : TestifyAttribute {
		/// <summary>
		/// The relative order in which this filter will be run. The lower
		/// this value, the earlier the filter will be run in relation to
		/// other ordered filters.
		/// </summary>
		public virtual int Order { get; set; }
	}
}