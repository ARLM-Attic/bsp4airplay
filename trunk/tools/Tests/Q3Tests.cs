using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;

namespace Tests
{
	[TestFixture]
	public class Q3Tests
	{
		[Test]
		public void TestQ3()
		{
			var doc = BspDocument.Load(@"..\data\maps\q3shw18.bsp");
		}
	}
}
