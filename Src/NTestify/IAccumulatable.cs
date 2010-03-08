using NTestify.Execution;

namespace NTestify {
	/// <summary>
	/// Represents an object that can accumulate tests
	/// </summary>
	/// <typeparam name="TContext">The accumulation context</typeparam>
	/// <typeparam name="TAccumulator">The test accumulator</typeparam>
	public interface IAccumulatable<TContext, TAccumulator>
		where TAccumulator : ITestAccumulator<TContext>
		where TContext : class {

		/// <summary>
		/// Gets the test accumulator
		/// </summary>
		TAccumulator Accumulator { get; }
	}
}