using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat;

namespace Bsp2AirplayAdapter
{
	public class Adapter
	{
		public static void Convert(string bspFilePath, string groupFilePath)
		{
			var doc = BspDocument.Load(bspFilePath);

		}
	}
}
