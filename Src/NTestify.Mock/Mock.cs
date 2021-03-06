﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NTestify.Mock.Expressions;
using System.Linq.Expressions;

namespace NTestify.Mock {

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers {
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);

		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
	}

	/// <summary>
	/// A mock of a class or interface
	/// </summary>
	/// <typeparam name="T">The type to mock</typeparam>
	public class Mock<T> : IHideObjectMembers where T : class {
		private readonly object[] constructorArguments;

		private readonly IList<IExpectation> expectations;
		private static readonly MockObjectBuilder Builder = new MockObjectBuilder();

		/// <summary>
		/// Initializes a new mock object
		/// </summary>
		public Mock(params object[] constructorArguments) {
			this.constructorArguments = constructorArguments;
			expectations = new List<IExpectation>();
		}

		/// <summary>
		/// Creates an expectation for an invocation that has a return value
		/// </summary>
		/// <typeparam name="TReturn">The invocation's return value type</typeparam>
		/// <param name="invocation">The expected invocation. This can be a property getter or a method invocation that has a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T, TReturn> Expects<TReturn>(Expression<Func<T, TReturn>> invocation) {
			var expectation = new Expectation<T, TReturn>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T, TReturn>(expectation);
		}

		/// <summary>
		/// Creates an expectation for an invocation without a return value
		/// </summary>
		/// <param name="invocation">The expected invocation. This can be a property setter or a method invocation that does not have a return value.</param>
		/// <returns>A mock expression</returns>
		public ExpectExpression<T> Expects(Expression<Action<T>> invocation) {
			var expectation = new Expectation<T>(invocation);
			expectations.Add(expectation);
			return new ExpectExpression<T>(expectation);
		}

		/// <summary>
		/// Verifies that the expectations were met
		/// </summary>
		public void Verify() {
			var errors = new List<string>();
			foreach (var expectation in expectations) {
				if (expectation.ActualInvocationCount != expectation.ExpectedInvocationCount) {
					errors.Add(
						string.Format(
							"{0} was expected to be invoked {1} {2}, but was actually invoked {3} {4}",
							expectation,
							expectation.ExpectedInvocationCount,
							expectation.ExpectedInvocationCount == 1 ? "time" : "times",
							expectation.ActualInvocationCount,
							expectation.ActualInvocationCount == 1 ? "time" : "times"
						)
					);
				}
			}

			if (errors.Count > 0) {
				throw new MockVerificationException(
					string.Format(
						"Expectations were not met for {0}:{1}",
						ToString(),
						errors.Aggregate((current, error) => current + "\n  " + error)
					)
				);
			}
		}

		/// <summary>
		/// Gets the actual object instance for this mock
		/// </summary>
		public T Object { get { return Builder.Build<T>(expectations, constructorArguments); } }

		public override string ToString() {
			return GetType().GetFriendlyName();
		}
	}

	public class MockVerificationException : Exception {
		public MockVerificationException(string message) : base(message) { }
	}

}