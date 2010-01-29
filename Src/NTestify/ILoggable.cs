namespace NTestify {
	/// <summary>
	/// Indicates that this class is able to log messages
	/// </summary>
	public interface ILoggable {
		/// <summary>
		/// Sets the logger for this object
		/// </summary>
		/// <param name="logger">The logger to use for logging (derp derp)</param>
		void SetLogger(ILogger logger);
	}
}