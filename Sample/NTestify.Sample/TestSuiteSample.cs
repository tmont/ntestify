using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTestify.Sample {
	[Test]
	public class TestSuiteSample {

		[Test]
		public void Should_pass() {
			Assert.True(true);
			Assert.Equal(1, 1);
		}

		[Test]
		public void Should_fail() {
			throw new TestAssertionException();
		}

		[Test]
		public void Should_err() {
			throw new Exception("I AM ERROR.");
		}

		[Ignore(Reason = "This test sux!")]
		public void Should_be_ignored() {

		}

		[ExpectedException(typeof(Exception), ExpectedMessage = "yay!")]
		public void Should_throw_exception_and_win() {
			throw new Exception("yay!");
		}

	}

	public class TestSuiteSample2 {
		[Test]
		public void LollerSk8() { }
	}
}
