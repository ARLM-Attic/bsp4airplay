using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class Cb4aLevelVBSubcluster : CIwParseable
	{
		public List<int> Indices = new List<int>();
		public int Material=0;
		public CIwVec3 SpherePos;
		public int SphereR;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("material", Material);
			writer.WriteVec3("sphere_pos", SpherePos);
			writer.WriteKeyVal("sphere_r", SphereR);
			writer.WriteKeyVal("num_indices", Indices.Count);
			foreach (var i in Indices)
				writer.WriteKeyVal("t", i);
		}
	}
}
