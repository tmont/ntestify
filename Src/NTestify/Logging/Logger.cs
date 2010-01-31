using System.IO;
using log4net;
using log4net.Config;

namespace NTestify.Logging {
	public class Logger : ILogger {

		private readonly ILog logger;

		public Logger(FileInfo file, string name) {
			XmlConfigurator.Configure(file);
			logger = LogManager.GetLogger(name);
		}

		public void Debug(object message) {
			logger.Debug(message);
		}

		public void Warn(object message) {
			logger.Warn(message);
		}

		public void Error(object message) {
			logger.Error(message);
		}

		public void Info(object message){
			logger.Info(message);
		}
	}
}