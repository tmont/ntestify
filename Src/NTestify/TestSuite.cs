using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTestify.Logging;

namespace NTestify {

	public interface ITestSuite : ITest {
		ITestSuite Add(ITest test);
	}

	public class TestSuite : ITestSuite, ILoggable {
		private ILogger logger;
		private readonly IList<ITest> tests;

		public IEnumerable<ITest> Tests { get { return tests; } }

		/// <summary>
		/// Event that fires before a test suite runs
		/// </summary>
		public event Action<ExecutionContext> BeforeTestSuiteRunEvent;
		/// <summary>
		/// Event that fires after a test suite runs
		/// </summary>
		public event Action<ExecutionContext, TestMethodResult> AfterTestSuiteRunEvent;
		/// <summary>
		/// Event that fires if a test suite is ignored. A test suite is considered to
		/// be ignored when all of its tests are ignored, or the suite itself is marked
		/// as ignored.
		/// </summary>
		public event Action<ExecutionContext, TestMethodResult> TestSuiteIgnoredEvent;
		/// <summary>
		/// Event that fires when a test suite passes. A test suite is considered to have
		/// passed when all of its tests pass or are ignored.
		/// </summary>
		public event Action<ExecutionContext, TestMethodResult> TestSuitePassedEvent;
		/// <summary>
		/// Event that fires when a test suite fails. A test suite is considered to have
		/// failed when any of its tests fail or err.
		/// </summary>
		public event Action<ExecutionContext, TestMethodResult> TestSuiteFailedEvent;

		public TestSuite() {
			logger = new NullLogger();
			tests = new List<ITest>();

			BeforeTestSuiteRunEvent += OnBeforeTestSuiteRun;
			AfterTestSuiteRunEvent += OnAfterTestSuiteRun;
			TestSuiteIgnoredEvent += OnTestSuiteIgnored;
			TestSuiteFailedEvent += OnTestSuiteFailed;
			TestSuitePassedEvent += OnTestSuitePassed;
		}

		#region Default event handlers
		/// <summary>
		/// Default TestSuitePassedEvent handler
		/// </summary>
		protected virtual void OnTestSuitePassed(ExecutionContext executionContext, TestMethodResult result){
			logger.Debug("Test suite passed");
		}

		/// <summary>
		/// Default TestSuiteFailed handler
		/// </summary>
		protected virtual void OnTestSuiteFailed(ExecutionContext executionContext, TestMethodResult result) {
			logger.Debug("Test suite failed");
		}

		/// <summary>
		/// Default TestSuiteIgnoredEvent handler
		/// </summary>
		protected virtual void OnTestSuiteIgnored(ExecutionContext executionContext, TestMethodResult result) {
			logger.Debug("Test suite ignored");
		}

		/// <summary>
		/// Default AfterTestSuiteRunEvent handler
		/// </summary>
		protected virtual void OnAfterTestSuiteRun(ExecutionContext executionContext, TestMethodResult result) {
			logger.Debug("After test suite");
		}

		/// <summary>
		/// Default BeforeTestSuiteRunEvent handler
		/// </summary>
		protected virtual void OnBeforeTestSuiteRun(ExecutionContext executionContext) {
			logger.Debug("Before test suite");
		}
		#endregion

		protected virtual TestSuiteResult CreateTestResult(){
			var result = new TestSuiteResult(this);
			result.SetLogger(logger);
			return result;
		}

		public void Run(ExecutionContext executionContext) {
			executionContext.Test = this;
			var result = CreateTestResult();

			result.StartTime = DateTime.Now;
			result.Status = TestStatus.Running;
			foreach (var testMethod in Tests) {
				var methodContext = new ExecutionContext {
					Instance = executionContext.Instance,
				};

				testMethod.Run(methodContext);
				result.AddResult(methodContext.Result);
			}
			result.EndTime = DateTime.Now;

			SetResultStatus(result);
			executionContext.Result = result;
		}

		protected static void SetResultStatus(TestSuiteResult result){
			if (result.ErredTests.Any()) {
				result.Status = TestStatus.Error;
			} else if (result.FailedTests.Any()) {
				result.Status = TestStatus.Fail;
			} else if (!result.PassedTests.Any() && result.IgnoredTests.Any()) {
				result.Status = TestStatus.Ignore;
			} else {
				result.Status = TestStatus.Pass; //yay
			}
		}

		public ITestSuite Add(ITest test) {
			tests.Add(test);
			return this;
		}

		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}

	}
}
