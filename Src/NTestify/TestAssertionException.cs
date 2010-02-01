﻿using System;

namespace NTestify {
	/// <summary>
	/// Thrown when an assertion fails
	/// </summary>
	public class TestAssertionException : Exception {
		public TestAssertionException() {
			
		}

		public TestAssertionException(string message) : base(message){
			
		}
	}
}