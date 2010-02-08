using NUnit.Framework;
using NTestify.Mock;
using Ass = NUnit.Framework.Assert;

namespace NTestify.Tests {
	[TestFixture]
	public class MockObjectTest {

		[NUnit.Framework.Test]
		public void Do_stuff() {
			var x = 0;
			var expected = new object();
			var mock = new Mock<ClassToMock>();
			mock
				.Expects(c => c.GetStuff())
				.WillExecuteCallback(c => x = 2)
				.AndReturn(expected);

			

			var actual = mock.Object.GetStuff();
			Ass.That(x, Is.EqualTo(2));
			Ass.That(expected, Is.SameAs(actual));
		}


	}

	internal class ClassToMock {
		public int Count { get; set; }

		public void DoStuff() {

		}

		public object GetStuff() {
			return this;
		}
	}
}