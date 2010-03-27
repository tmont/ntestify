using System.Collections.Generic;
using System.IO;

namespace NTestify.ConsoleRunner {
	public class ConsoleTestSuite : TestSuite {
		private readonly IEnumerable<FileInfo> files;

		public ConsoleTestSuite(IEnumerable<FileInfo> files) {
			this.files = files;
		}

		public IEnumerable<FileInfo> Files { get { return files; } }
	}
}