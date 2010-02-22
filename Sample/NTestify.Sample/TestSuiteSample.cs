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

		[Test(Category = "test")]
		public void Should_fail() {
			Assert.Equal(0, 1);
		}

		[Test(Name = "foo")]
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
		[Test(Category = "test")]
		public void LollerSk8() { }
	}
}
