using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTestify {
	internal sealed class FilteredTrace : StackTrace {
		private readonly IEnumerable<StackFrame> stackFrames;

		public FilteredTrace(Exception exception) {
			var ntestifyAssembly = typeof(ITest).Assembly;
			var frames = new StackTrace(exception, true).GetFrames() ?? Enumerable.Empty<StackFrame>();
			stackFrames = frames.Where(frame => frame.GetMethod().DeclaringType.Assembly != ntestifyAssembly);
		}

		public override StackFrame[] GetFrames() {
			return stackFrames.ToArray();
		}

		public override int FrameCount {
			get {
				return stackFrames.Count();
			}
		}

		public override StackFrame GetFrame(int index) {
			return stackFrames.ElementAt(index);
		}

		public override string ToString() {
			var trace = new StringBuilder();
			foreach (var frame in stackFrames) {
				trace.AppendFormat("at {0} in {1}:{2}", FormatMethodCall(frame.GetMethod()), frame.GetFileName(), frame.GetFileLineNumber());
				trace.AppendLine();
			}

			return trace.ToString();
		}

		private static string FormatMethodCall(MethodBase method) {
			var methodCall = new StringBuilder();
			methodCall.Append(method.DeclaringType.GetFriendlyName());
			methodCall.Append("." + method.Name + "(");

			var parameters = method.GetParameters();
			if (parameters.Length > 0) {
				methodCall.Append(parameters
					.Select(p => p.GetType().GetFriendlyName() + " " + p.Name)
					.Aggregate((current, next) => current + ", " + next)
				);
			}
			methodCall.Append(")");
			return methodCall.ToString();
		}
	}
}