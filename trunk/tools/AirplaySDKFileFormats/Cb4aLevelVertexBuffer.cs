using System;
using System.Globalization;
using System.Collections.Generic;
using AirplaySDKFileFormats.Model;

namespace AirplaySDKFileFormats
{
	public class Cb4aLevelVertexBuffer : CIwParseable
	{
		public List<LevelVBItem> vb = new List<LevelVBItem>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_vertices", vb.Count);
			foreach (var l in vb)
			{
				l.WrtieToStream(writer);
			}
		}
	}
}
