namespace NTestify.Logging {
	/// <summary>
	/// Boring logging class that does nothing. Its only purpose is
	/// to ease the pain of null reference exceptions.
	/// </summary>
	internal class NullLogger : ILogger {
		public void Debug(object message) { }
		public void Warn(object message) { }
		public void Error(object message) { }
		public void Info(object message) { }
	}
}