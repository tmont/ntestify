namespace NTestify.Logging {
	/// <summary>
	/// Indicates that this class is able to log messages
	/// </summary>
	public interface ILoggable<T> {
		/// <summary>
		/// Sets the logger for this object
		/// </summary>
		/// <param name="logger">The logger to use for logging (derp derp)</param>
		T SetLogger(ILogger logger);
	}
}