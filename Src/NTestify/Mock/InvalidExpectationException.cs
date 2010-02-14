using System;

namespace NTestify.Mock {
	internal class InvalidExpectationException : Exception {
		public InvalidExpectationException(string message) : base(message) { }
	}
}