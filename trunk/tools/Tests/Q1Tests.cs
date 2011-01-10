using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using System.IO;
using System.Globalization;
using Bsp2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class Q1Tests
	{
		[Test]
		public void TestQ1()
		{
			//(new Adapter()).Convert(@"..\data\maps\sg0503.bsp", @"..\data\maps\sg0503.group");
		}
		[Test]
		public void TestHL1()
		{
			//(new Adapter()).Convert(@"..\data\maps\hldemo1.bsp", @"..\data\maps\hldemo1.group");
			(new Adapter()).Convert(@"..\data\maps\samplebox.bsp", @"..\data\maps\samplebox.group");
			//(new Adapter()).Convert(@"..\data\maps\madcrabs.bsp", @"..\data\maps\madcrabs.group");
		}
	}
}