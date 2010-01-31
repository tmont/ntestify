using System.IO;
using log4net;
using log4net.Config;

namespace NTestify.Logging {
	/// <summary>
	/// Logger that uses log4net as its underlying logging mechanism
	/// </summary>
	public class Log4NetLogger : ILogger {

		private readonly ILog logger;

		/// <param name="file">The XML file to use for configuration</param>
		/// <param name="name">The name of the logger to retrieve</param>
		public Log4NetLogger(FileInfo file, string name) {
			XmlConfigurator.Configure(file);
			logger = LogManager.GetLogger(name);
		}

		/// <summary>
		/// Logs a debug message
		/// </summary>
		public void Debug(object message) {
			logger.Debug(message);
		}

		/// <summary>
		/// Logs a warning message
		/// </summary>
		public void Warn(object message) {
			logger.Warn(message);
		}

		/// <summary>
		/// Logs an error message
		/// </summary>
		public void Error(object message) {
			logger.Error(message);
		}

		/// <summary>
		/// Logs an informational message
		/// </summary>
		public void Info(object message){
			logger.Info(message);
		}
	}
}