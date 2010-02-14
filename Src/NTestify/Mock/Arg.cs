using System;

namespace NTestify.Mock {
	public static class Arg {

		public static TArg Any<TArg>() {
			throw new InvalidOperationException("This method should only be used in expression trees and should never be invoked");
		}

	}

	public interface ISpecialArgument {
		bool Matches(object obj);
	}

	public sealed class AnyArg : ISpecialArgument {
		private readonly Type type;

		public AnyArg(Type type) {
			this.type = type;
		}

		public bool Matches(object obj) {
			return obj.GetType() == type;
		}
	}
}