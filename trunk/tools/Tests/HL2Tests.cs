using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using Bsp2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class HL2Tests
	{
		[Test]
		public void TestHL2()
		{
			(new Adapter()).Convert(@"..\data\maps\leonHL2_1.bsp", @"..\data\maps\leonHL2_1.group");
		}
	}
}
