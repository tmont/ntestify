using System;
using System.Collections.Generic;
using System.Linq;
using NTestify.Logging;

namespace NTestify {
	/// <summary>
	/// Represents a collection (i.e. "suite") of tests that will be run
	/// sequentially and then aggregated into one test result upon completion
	/// </summary>
	public class TestSuite : ITestSuite, ILoggable {
		private ILogger logger;
		private readonly List<ITest> tests;

		/// <summary>
		/// All tests that have been added to this suite
		/// </summary>
		public IEnumerable<ITest> Tests { get { return tests; } }

		/// <summary>
		/// Event that fires before a test suite runs
		/// </summary>
		public event Action<ExecutionContext> BeforeTestSuiteRunEvent;
		/// <summary>
		/// Event that fires after a test suite runs
		/// </summary>
		public event Action<ExecutionContext> AfterTestSuiteRunEvent;
		/// <summary>
		/// Event that fires if a test suite is ignored. A test suite is considered to
		/// be ignored when all of its tests are ignored, or the suite itself is marked
		/// as ignored.
		/// </summary>
		public event Action<ExecutionContext> TestSuiteIgnoredEvent;
		/// <summary>
		/// Event that fires when a test suite passes. A test suite is considered to have
		/// passed when all of its tests pass or at least one test passes and the rest are
		/// ignored.
		/// </summary>
		public event Action<ExecutionContext> TestSuitePassedEvent;
		/// <summary>
		/// Event that fires when a test suite fails. A test suite is considered to have
		/// failed when any of its tests fail or err.
		/// </summary>
		public event Action<ExecutionContext> TestSuiteFailedEvent;

		public TestSuite() {
			logger = new NullLogger();
			tests = new List<ITest>();

			BeforeTestSuiteRunEvent += OnBeforeTestSuiteRun;
			AfterTestSuiteRunEvent += OnAfterTestSuiteRun;
			TestSuiteIgnoredEvent += OnTestSuiteIgnored;
			TestSuiteFailedEvent += OnTestSuiteFailed;
			TestSuitePassedEvent += OnTestSuitePassed;
		}

		public TestSuite(ITest test) : this() {
			if (test != null) {
				tests.Add(test);
			}
		}

		public TestSuite(IEnumerable<ITest> tests) : this() {
			if (tests != null) {
				this.tests.AddRange(tests);
			}
		}

		#region Default event handlers
		/// <summary>
		/// Default TestSuitePassedEvent handler
		/// </summary>
		protected virtual void OnTestSuitePassed(ExecutionContext executionContext) {
			logger.Debug("Test suite passed");
		}

		/// <summary>
		/// Default TestSuiteFailed handler
		/// </summary>
		protected virtual void OnTestSuiteFailed(ExecutionContext executionContext) {
			logger.Debug("Test suite failed");
		}

		/// <summary>
		/// Default TestSuiteIgnoredEvent handler
		/// </summary>
		protected virtual void OnTestSuiteIgnored(ExecutionContext executionContext) {
			logger.Debug("Test suite ignored");
		}

		/// <summary>
		/// Default AfterTestSuiteRunEvent handler
		/// </summary>
		protected virtual void OnAfterTestSuiteRun(ExecutionContext executionContext) {
			logger.Debug("After test suite");
		}

		/// <summary>
		/// Default BeforeTestSuiteRunEvent handler
		/// </summary>
		protected virtual void OnBeforeTestSuiteRun(ExecutionContext executionContext) {
			logger.Debug("Before test suite");
		}
		#endregion

		/// <summary>
		/// Creates a test result for use by the execution context
		/// </summary>
		protected virtual TestSuiteResult CreateTestResult() {
			var result = new TestSuiteResult(this);
			result.SetLogger(logger);
			return result;
		}

		/// <inheritdoc/>
		public void Run(ExecutionContext executionContext) {
			executionContext.Test = this;
			BeforeTestSuiteRunEvent.Invoke(executionContext);

			executionContext.Result = CreateTestResult();
			executionContext.Result.StartTime = DateTime.Now;
			executionContext.Result.Status = TestStatus.Running;
			foreach (var test in Tests) {
				var methodContext = new ExecutionContext {
					Instance = executionContext.Instance,
				};

				test.Run(methodContext);
				((TestSuiteResult)executionContext.Result).AddResult(methodContext.Result);
			}
			executionContext.Result.EndTime = DateTime.Now;

			SetResultStatusAndInvokeEvent(executionContext);
			AfterTestSuiteRunEvent.Invoke(executionContext);
		}

		/// <summary>
		/// Sets the result status based on the inner tests' results
		/// </summary>
		protected void SetResultStatusAndInvokeEvent(ExecutionContext executionContext) {
			var result = (TestSuiteResult)executionContext.Result;
			if (result.Results.Any(r => r.Status == TestStatus.Error || r.Status == TestStatus.Fail)) {
				result.Status = TestStatus.Fail;
				TestSuiteFailedEvent.Invoke(executionContext);
			} else if (!result.IsEmpty && result.Results.All(r => r.Status == TestStatus.Ignore)) {
				result.Status = TestStatus.Ignore;
				TestSuiteIgnoredEvent.Invoke(executionContext);
			} else {
				result.Status = TestStatus.Pass; //yay
				TestSuitePassedEvent.Invoke(executionContext);
			}
		}

		///<inheritdoc/>
		public ITestSuite AddTest(ITest test) {
			tests.Add(test);
			return this;
		}

		/// <inheritdoc/>
		public void SetLogger(ILogger logger) {
			this.logger = logger;
		}

	}

	

}
