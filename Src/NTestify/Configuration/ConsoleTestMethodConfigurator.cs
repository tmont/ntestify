﻿using System;

namespace NTestify.Configuration {
	/// <summary>
	/// The standard console output: "." means a test passed, "I" means a test was
	/// ignored, "F" means a test failed, and "E" means a test erred.
	/// </summary>
	public class ConsoleTestMethodConfigurator : VariableVerbosityConfigurator<ReflectedTestMethod> {

		private int currentLineLength;
		/// <summary>
		/// The default maximum number of columns per line
		/// </summary>
		public const int DefaultMaxLineLength = 50;
		private const int MinLineLength = 10;

		/// <summary>
		/// Creates a console configurator with DefaultMaxLineLength for the
		/// maximum line length
		/// </summary>
		public ConsoleTestMethodConfigurator() : this(VerbosityLevel.Default, DefaultMaxLineLength) { }

		/// <param name="verbosityLevel">The verbosity level</param>
		/// <param name="maxLineLength">The maximum number of columns per line</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public ConsoleTestMethodConfigurator(VerbosityLevel verbosityLevel, int maxLineLength)
			: base(verbosityLevel) {
			if (maxLineLength < MinLineLength) {
				throw new ArgumentOutOfRangeException("maxLineLength", maxLineLength, "Value must be greater than or equal to " + MinLineLength);
			}

			MaxLineLineLength = maxLineLength;
		}

		/// <summary>
		/// Gets or sets the maximum line length
		/// </summary>
		public int MaxLineLineLength { get; private set; }

		private void WriteResult(char indicator) {
			if (currentLineLength == 0) {
				Console.Write("  ");
				currentLineLength = 2;
			}

			Console.Write(indicator);
			currentLineLength = (currentLineLength + 1) % MaxLineLineLength;
		}

		protected override void ConfigureTest(ReflectedTestMethod test) {
			test.OnError += context => WriteResult('E');
			test.OnPass += context => WriteResult('.');
			test.OnFail += context => WriteResult('F');
			test.OnIgnore += context => WriteResult('I');
		}
	}
}