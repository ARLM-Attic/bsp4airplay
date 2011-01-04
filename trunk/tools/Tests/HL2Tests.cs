using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;

namespace Tests
{
	[TestFixture]
	public class HL2Tests
	{
		[Test]
		public void TestHL2()
		{
			var doc = BspDocument.Load(@"..\data\maps\leonHL2_1.bsp");
		}
	}
}
