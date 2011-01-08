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
		//CIwModel model;
		LevelVBWriter writer;
		public void Convert(BspDocument bsp, CIwResGroup group)
		{
			this.group = group;
			this.level = new Cb4aLevel();
			level.Name = bsp.Name;
			writer = new LevelVBWriter(level);
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
					leaf.Cluster = WriteVB(bspTreeLeaf, writer);
					//if (model == null)
					//{
					//    model = new CIwModel() { Name = l.Name };
					//    group.AddRes(model);
					//}
					//leaf.Model = model.Name;

					////leaf.Model = l.Name + "/leaf" + i;
					////var geo = new CIwModel() { Name = leaf.Model };
					////geo.Mesh.Name = geo.Name;
					////group.AddRes(geo);

					//var mesh = new CMesh() { Name = model.Name + "/leaf" + i };
					//leaf.Mesh = model.ModelBlocks.Count;
					//model.ModelBlocks.Add(mesh);

					//var modelWriter = new ModelWriter(mesh);
					//WriteGeometry(bspTreeLeaf.Geometry, modelWriter);
				}
				foreach (var vis in bspTreeLeaf.VisibleLeaves)
				{
					leaf.VisibleLeaves.Add(AddTreeLeaf(l, vis));
				}
			}
			return i;
		}

		private int WriteVB(BspTreeLeaf bspTreeLeaf, LevelVBWriter writer)
		{
			if (bspTreeLeaf.Geometry == null)
				return -1;
			if (bspTreeLeaf.Geometry.Faces.Count == 0)
				return -1;


			var clusterIndex = level.clusters.Count;
			var cluster = new Cb4aLevelVBCluster();
			level.clusters.Add(cluster);

			Dictionary<BspTexture, bool> materialMap = new Dictionary<BspTexture, bool>();
			foreach (var f in bspTreeLeaf.Geometry.Faces)
			{
				materialMap[f.Texture] = true;
			}
			foreach (BspTexture t in materialMap.Keys)
			{
				var sub = new Cb4aLevelVBSubcluster();
				cluster.Subclusters.Add(sub);
				
				foreach (var f in bspTreeLeaf.Geometry.Faces)
				{
					if (f.Texture == t)
					{
						//TODO: slice face into more faces
						while (f.Vertex0.UV0.X >= 8 || f.Vertex1.UV0.X >= 8 || f.Vertex2.UV0.X >= 8)
						{
							f.Vertex0.UV0.X -= 1; if (f.Vertex0.UV0.X < -8) f.Vertex0.UV0.X = -8;
							f.Vertex1.UV0.X -= 1; if (f.Vertex1.UV0.X < -8) f.Vertex1.UV0.X = -8;
							f.Vertex2.UV0.X -= 1; if (f.Vertex2.UV0.X < -8) f.Vertex2.UV0.X = -8;
						}
						while (f.Vertex0.UV0.Y >= 8 || f.Vertex1.UV0.Y >= 8 || f.Vertex2.UV0.Y >= 8)
						{
							f.Vertex0.UV0.Y -= 1; if (f.Vertex0.UV0.Y < -8) f.Vertex0.UV0.Y = -8;
							f.Vertex1.UV0.Y -= 1; if (f.Vertex1.UV0.Y < -8) f.Vertex1.UV0.Y = -8;
							f.Vertex2.UV0.Y -= 1; if (f.Vertex2.UV0.Y < -8) f.Vertex2.UV0.Y = -8;
						}
						sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex2)));
						sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex1)));
						sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex0)));
					}
				}
				
			}
			
			return clusterIndex;
		}

		private LevelVBItem GetLevelVBItem(BspGeometryVertex bspGeometryVertex)
		{
			return new LevelVBItem() { Position = GetVec3(bspGeometryVertex.Position),
									   Normal = GetVec3Fixed(bspGeometryVertex.Normal),
									   UV0 = GetVec2Fixed(bspGeometryVertex.UV0),
									   UV1 = GetVec2Fixed(bspGeometryVertex.UV1),
									   Colour = CIwColour.White
			};
		}

		private void WriteGeometry(BspGeometry bspGeometry, ModelWriter modelWriter)
		{
			foreach (var face in bspGeometry.Faces)
			{
				WriteMaterial(face.Texture, modelWriter);
				var surface = modelWriter.GetSurfaceIndex(face.Texture.Name);
				CIwVec2 v0uv0 = GetVec2Fixed(face.Vertex0.UV0);
				CIwVec2 v1uv0 = GetVec2Fixed(face.Vertex1.UV0);
				CIwVec2 v2uv0 = GetVec2Fixed(face.Vertex2.UV0);
				while (v0uv0.x > 32767 || v1uv0.x > 32767 || v2uv0.x > 32767)
				{
					v0uv0.x = v0uv0.x-AirplaySDKMath.IW_GEOM_ONE;
					v1uv0.x = v1uv0.x-AirplaySDKMath.IW_GEOM_ONE;
					v2uv0.x = v2uv0.x-AirplaySDKMath.IW_GEOM_ONE;
				}
				while (v0uv0.y > 32767 || v1uv0.y > 32767 || v2uv0.y > 32767)
				{
					v0uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
					v1uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
					v2uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
				}
				BspGeometryVertex vertex = face.Vertex0;
				var v0 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), v0uv0, GetVec2Fixed(vertex.UV1), CIwColour.White);
				vertex = face.Vertex1;
				var v1 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), v1uv0, GetVec2Fixed(vertex.UV1), CIwColour.White);
				vertex = face.Vertex2;
				var v2 = modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), v2uv0, GetVec2Fixed(vertex.UV1), CIwColour.White);
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
		private CIwColour GetColour(object col)
		{
			return new CIwColour() { };
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
