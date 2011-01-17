using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aLeaf : Cb4aTreeElement
	{
		public List<int> VisibleLeaves = new List<int>();
		public List<Ib4aCollider> Colliders = new List<Ib4aCollider>();
		public string Model;
		public int Mesh = -1;
		public int Cluster = -1;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			
			writer.WriteString("model", Model);
			if (Model != null)
				writer.WriteKeyVal("mesh", Mesh);
			if (Cluster >= 0)
				writer.WriteKeyVal("cluster", Cluster);
			writer.WriteKeyVal("num_visible_leaves", VisibleLeaves.Count);
			foreach (var l in VisibleLeaves)
			{
				writer.WriteKeyVal("visible_leaf", l);
			}
			foreach (var l in Colliders)
			{
				((CIwParseable)l).WrtieToStream(writer);
			}
		}
	}
}
