using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aLeaf : Cb4aTreeElement
	{
		public List<int> VisibleLeaves = new List<int>();
		public List<int> Clusters = new List<int>();
		public List<Ib4aCollider> Colliders = new List<Ib4aCollider>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			
			if (Clusters.Count > 0)
			{
				writer.WriteKeyVal("num_visible_clusters", Clusters.Count);
				writer.BeginWriteLine();
				writer.Write("visible_clusters");
				foreach (var l in Clusters)
				{
					writer.Write(string.Format(" {0}", l));
				}
				writer.EndWriteLine();
			}
			if (VisibleLeaves.Count > 0)
			{
				writer.WriteKeyVal("num_visible_leaves", VisibleLeaves.Count);
				writer.BeginWriteLine();
				writer.Write("visible_leaves");
				foreach (var l in VisibleLeaves)
				{
					writer.Write(string.Format(" {0}", l));
				}
				writer.EndWriteLine();
			}
			foreach (var l in Colliders)
			{
				((CIwParseable)l).WrtieToStream(writer);
			}
		}
	}
}
