﻿using System.Collections.Generic;

namespace NTestify {
	/// <summary>
	/// Interface for test suites, which represent a container for
	/// many tests
	/// </summary>
	public interface ITestSuite : ITest {
		/// <summary>
		/// Adds a test to the suite. Note that the test can also
		/// be another test suite
		/// </summary>
		ITestSuite AddTest(ITest test);

		/// <summary>
		/// Adds a range of tests to the suite
		/// </summary>
		ITestSuite AddTests(IEnumerable<ITest> listOfTests);
	}
}