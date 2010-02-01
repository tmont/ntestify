namespace NTestify.Logging {
	/// <summary>
	/// Indicates that this class is able to log messages
	/// </summary>
	public interface ILoggable {
		/// <summary>
		/// Gets or sets the logger for this object
		/// </summary>
		ILogger Logger { get; set; }
	}
}