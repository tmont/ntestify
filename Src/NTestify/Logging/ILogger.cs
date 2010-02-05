namespace NTestify.Logging {
	/// <summary>
	/// Basic interface for logging
	/// </summary>
	public interface ILogger {
		/// <summary>
		/// Logs a debug message
		/// </summary>
		void Debug(object message);

		/// <summary>
		/// Logs a warning message
		/// </summary>
		void Warn(object message);

		/// <summary>
		/// Logs an error message
		/// </summary>
		void Error(object message);

		/// <summary>
		/// Logs an informational message
		/// </summary>
		void Info(object message);
	}
}