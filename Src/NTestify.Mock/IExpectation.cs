using System.Collections.Generic;
using System.Linq.Expressions;

namespace NTestify.Mock {
	public interface IExpectation {
		int ExpectedInvocationCount { get; set; }
		int ActualInvocationCount { get; }
		bool Matches(LambdaExpression expression);
		string MethodOrPropertyName { get; }
		IEnumerable<object> Arguments { get; }
	}
}