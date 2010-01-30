using NUnit.Framework;

namespace NTestify.Tests {
	public static class TestResultExtensionsForTesting {
		public static TTestResult CastTo<TTestResult>(this ITestResult result) where TTestResult : ITestResult {
			Assert.That(result, Is.InstanceOf(typeof(TTestResult)), string.Format("Test result expected to be of type {0}", typeof(TTestResult).GetType().FullName));
			return (TTestResult)result;
		}
	}
}