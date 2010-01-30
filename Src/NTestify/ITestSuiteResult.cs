using System;
using System.Collections.Generic;

namespace NTestify {
	public interface ITestSuiteResult : ITestResult {
		bool IsEmpty { get; }
		IEnumerable<ITestResult> Results { get; }
		IEnumerable<ITest> PassedTests { get; }
		IEnumerable<ITest> ErredTests { get; }
		IEnumerable<ITest> FailedTests { get; }
		IEnumerable<ITest> IgnoredTests { get; }
		IEnumerable<ITest> AllTests { get; }
		IEnumerable<Exception> AllErrors { get; }
		ITestSuiteResult AddResult(ITestResult result);
	}
}