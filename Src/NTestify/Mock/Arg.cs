using System;

namespace NTestify.Mock {
	public static class Arg {

		public static TArg Any<TArg>() {
			throw new InvalidOperationException("This method should only be used in expression trees and should never be invoked");
		}

		public static TArg SimilarTo<TArg>(TArg arg) {
			throw new InvalidOperationException("This method should only be used in expression trees and should never be invoked");
		}

	}

	public sealed class AnyArg {
		public Type Type { get; private set; }

		public AnyArg(Type type) {
			Type = type;
		}
	}
}