using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aLevel: CIwResource
	{
		public List<Cb4aNode> Nodes = new List<Cb4aNode>();
		public List<Cb4aLeaf> Leaves = new List<Cb4aLeaf>();
		public List<Cb4aEntity> Entities = new List<Cb4aEntity>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_leaves", Leaves.Count);
			foreach (var l in Leaves)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_nodes", Nodes.Count);
			foreach (var l in Nodes)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_entities", Entities.Count);
			foreach (var l in Entities)
			{
				l.WrtieToStream(writer);
			}
		}
	}
}
