using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat;
using AirplaySDKFileFormats;
using AirplaySDKFileFormats.Model;
using System.IO;

namespace Bsp2AirplayAdapter
{
	public class Adapter
	{
		Dictionary<BspTreeNode, int> nodeIndices = new Dictionary<BspTreeNode, int>();
		Dictionary<BspTreeLeaf, int> leafIndices = new Dictionary<BspTreeLeaf, int>();
		Dictionary<BspTexture, int> textureIndices = new Dictionary<BspTexture, int>();
		CIwResGroup group;
		Cb4aLevel level;
		public void Convert(BspDocument bsp, CIwResGroup group)
		{
			this.group = group;
			this.level = new Cb4aLevel();
			level.Name = bsp.Name;
			group.AddRes(level);

			foreach (var e in bsp.Entities)
			{
				level.Entities.Add(new Cb4aEntity() { classname = e.ClassName, origin = GetVec3(e.Origin), values = e.Values });
			}
			if (bsp.Tree != null)
				AddTreeElement(level, bsp.Tree);
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
				if (bspTreeLeaf.Geometry != null && bspTreeLeaf.Geometry.Faces.Count > 0)
				{
					leaf.Model = l.Name + "/leaf" + i;
					var geo = new CIwModel() { Name = leaf.Model };
					geo.Mesh.Name = geo.Name;
					var modelWriter = new ModelWriter(geo.Mesh);
					WriteGeometry(bspTreeLeaf.Geometry, modelWriter);
					group.AddRes(geo);
				}
				foreach (var vis in bspTreeLeaf.VisibleLeaves)
				{
					leaf.VisibleLeaves.Add(AddTreeLeaf(l, vis));
				}
			}
			return i;
		}

		private void WriteGeometry(BspGeometry bspGeometry, ModelWriter modelWriter)
		{
			foreach (var face in bspGeometry.Faces)
			{
				WriteMaterial(face.Texture, modelWriter);
				var surface = modelWriter.GetSurfaceIndex(face.Texture.Name);
				BspGeometryVertex vertex = face.Vertex0;
				var v0 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), GetVec2Fixed(vertex.UV0), GetVec2Fixed(vertex.UV1), CIwColour.White);
				vertex = face.Vertex1;
				var v1 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), GetVec2Fixed(vertex.UV0), GetVec2Fixed(vertex.UV1), CIwColour.White);
				vertex = face.Vertex2;
				var v2 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), GetVec2Fixed(vertex.UV0), GetVec2Fixed(vertex.UV1), CIwColour.White);
				modelWriter.AddTriangle(surface, v0, v1, v2);
			}
		}

		private void WriteMaterial(BspTexture bspTexture, ModelWriter modelWriter)
		{
			int i;
			if (textureIndices.TryGetValue(bspTexture, out i))
				return;
			textureIndices[bspTexture] = 0;
			var mtl = new CIwMaterial() { Name = bspTexture.Name, Texture0 = "./checkers.png" };
			//if (bspTexture is BspEmbeddedTexture)
			//{
			//    var filePath = Path.GetDirectoryName(level.SourceFileName);
			//    filePath = Path.Combine(filePath, level.Name);
			//    if (filePath != null)
			//        return null;
			//}
			//else if (bspTexture is BspTextureReference)
			//{
			//    var filePath = ((BspTextureReference)bspTexture).FilePath;
			//}
			group.AddRes(mtl);
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
		private CIwVec2 GetVec2Fixed(BspFileFormat.BspMath.Vector2 vector3)
		{
			return new CIwVec2(
				(int)(vector3.X * AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y * AirplaySDKMath.IW_GEOM_ONE)
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
