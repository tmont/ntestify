namespace NTestify.Logging {
	public interface ILogger {
		void Debug(object message);

		void Warn(object message);

		void Error(object message);
	}
}