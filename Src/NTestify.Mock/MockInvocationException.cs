using System;

namespace NTestify.Mock {
	public class MockInvocationException : Exception {
		public MockInvocationException(string message) : base(message) { }
	}
}