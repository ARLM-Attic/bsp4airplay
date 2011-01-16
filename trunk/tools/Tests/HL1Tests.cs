using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using System.IO;
using System.Globalization;
using Bsp2AirplayAdapter;
using DemFileFormat.Quake1;
using DemFileFormat;
using DemFileFormat.HL1;

namespace Tests
{
	[TestFixture]
	public class HL1Tests
	{
		[Test]
		public void TestHL1()
		{
			//(new Adapter()).Convert(@"..\data\maps\hldemo1.bsp", @"..\data\maps\hldemo1.group");
			(new Adapter()).Convert(@"..\data\maps\samplebox.bsp", @"..\data\maps\samplebox.group");
			//(new Adapter()).Convert(@"..\data\maps\madcrabs.bsp", @"..\data\maps\madcrabs.group");
		}
		[Test]
		public void TestCS_1_6()
		{
			(new Adapter()).Convert(@"..\data\maps\de_dust.bsp", @"..\data\maps\de_dust.group");
		}
		//[Test]
		public void TestDemo()
		{
			var res = new DemoDocument();

			using (var f = File.OpenRead(@"..\data\demos\dustdemo.dem"))
			{
				using (var b = new BinaryReader(f))
				{
					(new HL1DemoReader()).ReadDemo(b, res);
				}
			}
		}
	}
}
