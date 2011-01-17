using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using Bsp2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class Q3Tests
	{
		//[Test]
		public void TestQ3()
		{
			(new Adapter()).Convert(@"..\data\maps\q3shw18.bsp", @"..\data\maps\q3shw18.group");
		}
	}
}
