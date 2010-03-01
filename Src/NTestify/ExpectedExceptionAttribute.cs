using System;

namespace NTestify {
	/// <summary>
	/// Indicates that a test method is expected to throw an exception
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExpectedExceptionAttribute : TestifyAttribute {

		/// <summary>
		/// The expected exception message
		/// </summary>
		public string ExpectedMessage { get; set; }
		/// <summary>
		/// The type of exception that is expected to be thrown
		/// </summary>
		public Type ExpectedException { get; protected set; }

		/// <param name="expectedException">The type of exception that is expected to be thrown</param>
		public ExpectedExceptionAttribute(Type expectedException) {
			ExpectedException = expectedException;
		}
	}
}