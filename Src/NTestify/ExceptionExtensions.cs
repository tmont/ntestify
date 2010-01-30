using System;

namespace NTestify {
	public static class ExceptionExtensions {
		/// <summary>
		/// Traverses the exception hierarchy and retrieves the earliest raised exception
		/// </summary>
		public static Exception GetInnermostException(this Exception exception){
			while (exception.InnerException != null) {
				exception = exception.InnerException;
			}

			return exception;
		}
	}
}