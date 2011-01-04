using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;

namespace Tests
{
	[TestFixture]
	public class Q2Tests
	{
		[Test]
		public void TestQ2()
		{
			var doc = BspDocument.Load(@"..\data\maps\moo.bsp");
		}
	}
}
