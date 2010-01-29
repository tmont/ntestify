using System.Reflection;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Represents the result of a completed test method
	/// </summary>
	public class TestMethodResult : TestResult<TestMethod>, ILoggable {
		private ILogger logger;

		public TestMethodResult(TestMethod test) : base(test) {
			logger = new NullLogger();
		}

		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}
	}
}