using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat;
using AirplaySDKFileFormats;

namespace Bsp2AirplayAdapter
{
	public class Adapter
	{
		Dictionary<BspTreeNode, int> nodeIndices = new Dictionary<BspTreeNode, int>();
		Dictionary<BspTreeLeaf, int> leafIndices = new Dictionary<BspTreeLeaf, int>();

		public void Convert(BspDocument bsp, CIwResGroup group)
		{
			var l = new Cb4aLevel();
			l.Name = bsp.Name;
			group.AddRes(l);

			foreach (var e in bsp.Entities)
			{
				l.Entities.Add(new Cb4aEntity() { classname=e.ClassName, origin=GetVec3(e.Origin), values=e.Values });
			}
			if (bsp.Tree != null)
				AddTreeElement(l, bsp.Tree);
		}
		
		private int AddTreeElement(Cb4aLevel l, BspTreeElement bspTreeElement)
		{
			if (bspTreeElement is BspTreeNode)
				return AddTreeNode(l, (BspTreeNode)bspTreeElement);
			return AddTreeLeaf(l, (BspTreeLeaf)bspTreeElement);
		}

		private int AddTreeLeaf(Cb4aLevel l, BspTreeLeaf bspTreeLeaf)
		{
			int i;
			if (!leafIndices.TryGetValue(bspTreeLeaf, out i))
			{
				i = l.Leaves.Count;
				var leaf = new Cb4aLeaf() { Index = i };
				leafIndices[bspTreeLeaf] = i;
				l.Leaves.Add(leaf);
				foreach (var vis in bspTreeLeaf.VisibleLeaves)
				{
					leaf.VisibleLeaves.Add(AddTreeLeaf(l,vis));
				}
			}
			return i;
		}

		private int AddTreeNode(Cb4aLevel l, BspTreeNode bspTreeNode)
		{
			int i;
			if (!nodeIndices.TryGetValue(bspTreeNode, out i))
			{
				i = l.Nodes.Count;
				var node = new Cb4aNode() { Index = i };
				nodeIndices[bspTreeNode] = i;
				l.Nodes.Add(node);
				node.PlaneDistance = bspTreeNode.PlaneDistance;
				node.PlaneNormal = GetVec3Fixed(bspTreeNode.PlaneNormal);
				node.IsFrontLeaf = bspTreeNode.Front is BspTreeLeaf;
				node.Front = AddTreeElement(l, bspTreeNode.Front);
				node.IsBackLeaf = bspTreeNode.Back is BspTreeLeaf;
				node.Back = AddTreeElement(l, bspTreeNode.Back);
			}
			return i;
		}

		private CIwVec3 GetVec3Fixed(BspFileFormat.BspMath.Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X*AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y*AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Z*AirplaySDKMath.IW_GEOM_ONE)
				);
		}

		private CIwVec3 GetVec3(BspFileFormat.BspMath.Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X),
				(int)(vector3.Y),
				(int)(vector3.Z)
				);
		}
		
		public void Convert(string bspFilePath, string groupFilePath)
		{
			var doc = BspDocument.Load(bspFilePath);
			var group = new CIwResGroup();
			group.Name = doc.Name;
			Convert(doc, group);
			using (var w = new CTextWriter(groupFilePath))
			{
				group.WrtieToStream(w);
				w.Close();
			}
		}
	}
}
