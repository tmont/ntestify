using System.IO;
using log4net;
using log4net.Config;

namespace NTestify {
	public class Logger : ILogger {

		private readonly ILog logger;

		public Logger() : this(@"log4net.xml") {

		}

		public Logger(string file) {
			XmlConfigurator.Configure(new FileInfo(file));
			logger = LogManager.GetLogger("console-color");
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
	}
}