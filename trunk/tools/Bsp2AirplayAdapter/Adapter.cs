using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat;
using AirplaySDKFileFormats;
using AirplaySDKFileFormats.Model;
using System.IO;
using Atlasing;
using System.Drawing;

namespace Bsp2AirplayAdapter
{
	public class Adapter
	{
		Dictionary<BspTreeNode, int> nodeIndices = new Dictionary<BspTreeNode, int>();
		Dictionary<BspTreeLeaf, int> leafIndices = new Dictionary<BspTreeLeaf, int>();
		List<BspTreeLeaf> allLeaves = new List<BspTreeLeaf>();
		Dictionary<BspTexture, int> textureIndices = new Dictionary<BspTexture, int>();
		//Dictionary<BspTexture, int> lightmapIndices = new Dictionary<BspTexture, int>();
		CIwResGroup group;
		Cb4aLevel level;
		//CIwModel model;
		LevelVBWriter writer;
		BspTexture commonLightmap;
		public void Convert(BspDocument bsp, CIwResGroup group)
		{
			CollectAllLeaves(bsp.Tree);
			BuildLightmapAtlas(bsp);

			this.group = group;
			this.level = new Cb4aLevel();
			level.Name = bsp.Name;
			group.AddRes(new CIwTexture() { FilePath="../textures/checkers.png"});
			writer = new LevelVBWriter(level);
			group.AddRes(level);

			//level.Materials.Add(new Cb4aLevelMaterial() { Texture="checkers" });

			foreach (var e in bsp.Entities)
			{
				level.Entities.Add(new Cb4aEntity() { classname = e.ClassName, origin = GetVec3(e.Origin), values = e.Values });
			}
			AddLeaves(level);
			if (bsp.Tree != null)
				AddTreeNode(level, bsp.Tree);
		}

		private void AddLeaves(Cb4aLevel level)
		{
			for (int i=0; i<allLeaves.Count;++i)
			{
				var bspTreeLeaf = allLeaves[i];
				i = level.Leaves.Count;
				var ll = new Cb4aLeaf() { Index = i };
				level.Leaves.Add(ll);
				if (bspTreeLeaf.Geometry != null && bspTreeLeaf.Geometry.Faces.Count > 0)
				{
					ll.Cluster = WriteVB(bspTreeLeaf, writer);
				}
				foreach (var v in bspTreeLeaf.VisibleLeaves)
					ll.VisibleLeaves.Add(leafIndices[v]);
			}
		}
		private void BuildLightmapAtlas(BspDocument bsp)
		{
			Dictionary<Bitmap, bool> lightmaps = new Dictionary<Bitmap, bool>();
			CollectAllLightmaps(lightmaps);

			Atlas atlas = new Atlas();
			foreach (var l in lightmaps.Keys)
			{
				atlas.Add(l);
			}

			commonLightmap = new BspEmbeddedTexture() { Name = bsp.Name + "_lightmap", mipMaps = new Bitmap[] { atlas.Bitmap } };
			UpdateLightmap(commonLightmap, atlas);
			
		//    
		//    Bitmap Lightmap = new Bitmap(128,128);
		//    using (var g = Graphics.FromImage(Lightmap))
		//    {
		//        g.Clear(Color.Red);
		//    }
		//    BspEmbeddedTexture lm = new BspEmbeddedTexture() { Name = "lightmap", mipMaps = new Bitmap[1] { Lightmap } };
		//    foreach (var l in leaves)
		//    {
		//        if (l.Geometry != null)
		//            foreach (var f in l.Geometry.Faces)
		//                if (f.Lightmap != null)
		//                    f.Lightmap = lm;
		//    }
		}

		private void UpdateLightmap(BspTexture result, Atlas atlas)
		{
			foreach (var leaf in allLeaves)
			{
				if (leaf.Geometry == null)
					continue;
				Size dstSize = new Size(atlas.Bitmap.Width, atlas.Bitmap.Height);
				for (int i = 0; i < leaf.Geometry.Faces.Count; ++i)
				{
					var f = leaf.Geometry.Faces[i];
					if (f.Lightmap != null)
					{
						if (f.Lightmap.Equals(result))
							continue;
						var item = atlas.GetItem(((BspEmbeddedTexture)f.Lightmap).mipMaps[0]);
						var ff = new BspGeometryFace() { Texture = f.Texture, Lightmap = result };
						ff.Vertex0 = CorrectLightmapCoords(f.Vertex0, dstSize, item);
						ff.Vertex1 = CorrectLightmapCoords(f.Vertex1, dstSize, item);
						ff.Vertex2 = CorrectLightmapCoords(f.Vertex2, dstSize, item);
						leaf.Geometry.Faces[i] = ff;
					}
				}
			}
		}

		private BspGeometryVertex CorrectLightmapCoords(BspGeometryVertex src, Size dstSize, AtlasItem item)
		{
			BspGeometryVertex res = new BspGeometryVertex();
			res.Color = src.Color;
			res.Normal = src.Normal;
			res.Position = src.Position;
			res.UV0 = src.UV0;
			var size =item.Size;
			var position =item.Position;
			res.UV1.X = ((float)item.Position.X + src.UV1.X * (float)size.Width) / (float)dstSize.Width;
			res.UV1.Y = ((float)item.Position.Y + src.UV1.Y * (float)size.Height) / (float)dstSize.Height;
			return res;
		}

		private void CollectAllLeaves(BspTreeElement bspTreeElement)
		{
			if (bspTreeElement is BspTreeNode)
			{
				CollectAllLeaves(((BspTreeNode)bspTreeElement).Front);
				CollectAllLeaves(((BspTreeNode)bspTreeElement).Back);
				return;
			}
			var leaf = (BspTreeLeaf)bspTreeElement;
			int i;
			if (leafIndices.TryGetValue(leaf, out i))
				return;
			i = allLeaves.Count;
			allLeaves.Add(leaf);
			leafIndices[leaf] = i;
			foreach (var v in leaf.VisibleLeaves)
				CollectAllLeaves(v);
		}
		private void CollectAllLightmaps(Dictionary<Bitmap, bool> lightmaps)
		{
			foreach (var leaf in allLeaves)
			{
				if (leaf.Geometry == null)
					continue;
				foreach (var f in leaf.Geometry.Faces)
					if (f.Lightmap != null)
						lightmaps[((BspEmbeddedTexture)f.Lightmap).mipMaps[0]] = true;
			}
		}
		private int AddTreeNode(Cb4aLevel l, BspTreeElement bspTreeElement)
		{
			if (bspTreeElement is BspTreeNode)
				return AddTreeNode(l, (BspTreeNode)bspTreeElement);
			return AddTreeLeaf(l, (BspTreeLeaf)bspTreeElement);
		}

		private int AddTreeLeaf(Cb4aLevel l, BspTreeLeaf bspTreeLeaf)
		{
			return leafIndices[bspTreeLeaf];
			
		}
		void RegisterTexture(BspTexture t)
		{
			if (t == null)
				return;
			if (textureIndices.ContainsKey(t))
				return;
			textureIndices[t] = 0;
			string subfolder = "../textures/";
			//t.Name = t.Name;
			foreach (var c in Path.GetInvalidFileNameChars())
				t.Name = t.Name.Replace(c, '_');
			Bitmap bmp = null;
			if (t is BspEmbeddedTexture)
			{
				var embeddedTex = (BspEmbeddedTexture)t;
				if (embeddedTex.mipMaps != null && embeddedTex.mipMaps.Length > 0)
					bmp = embeddedTex.mipMaps[0];
			}
			group.AddRes(new CIwTexture() { FilePath = subfolder + t.Name + ".png", Bitmap = bmp });
		}

		private int WriteVB(BspTreeLeaf bspTreeLeaf, LevelVBWriter writer)
		{
			if (bspTreeLeaf.Geometry == null)
				return -1;
			if (bspTreeLeaf.Geometry.Faces.Count == 0)
				return -1;

			TessalateFaces(bspTreeLeaf.Geometry);

			var clusterIndex = level.clusters.Count;
			var cluster = new Cb4aLevelVBCluster();
			level.clusters.Add(cluster);

			writer.PrepareVertexBuffer(bspTreeLeaf.Geometry.Faces.Count * 3);
			cluster.VertexBuffer = level.VertexBuffers.Count - 1;
			Dictionary<int, bool> materialMap = new Dictionary<int, bool>();
			List<int> materialInices = new List<int>(bspTreeLeaf.Geometry.Faces.Count);
			foreach (var f in bspTreeLeaf.Geometry.Faces)
			{
				RegisterTexture(f.Texture);
				if (f.Lightmap != null && commonLightmap != f.Lightmap)
					throw new ApplicationException("not atlased lightmap");
				RegisterTexture(f.Lightmap);
				int matIndex = writer.WriteMaterial(new Cb4aLevelMaterial((f.Texture != null) ? f.Texture.Name : null, (f.Lightmap != null)?f.Lightmap.Name:null));
                materialInices.Add(matIndex);
				materialMap[matIndex] = true;
			}
			foreach (int t in materialMap.Keys)
			{
				var sub = new Cb4aLevelVBSubcluster();
				sub.Material = t;
				cluster.Subclusters.Add(sub);
				CIwVec3 mins = new CIwVec3(int.MaxValue, int.MaxValue, int.MaxValue);
				CIwVec3 maxs = new CIwVec3(int.MinValue, int.MinValue, int.MinValue);
				for (int i=0; i<materialInices.Count; ++i)
				{
					if (materialInices[i] == t)
					{
						var f = bspTreeLeaf.Geometry.Faces[i];

						//TODO: slice face into more faces
						BuildFace(writer, sub, ref mins, ref maxs, f);
					}
				}
				sub.SphereR = Math.Max(Math.Max(maxs.x - mins.x, maxs.y - mins.y), maxs.z - mins.z) / 2;
				if (sub.SphereR < 0)
				{
					sub.SpherePos = CIwVec3.g_Zero;
					sub.SphereR = 0;
				}
				else
				{
					sub.SpherePos = new CIwVec3((mins.x + maxs.x) / 2, (mins.y + maxs.y) / 2, (mins.z + maxs.z) / 2);
				}
			}
			
			return clusterIndex;
		}

		private void TessalateFaces(BspGeometry bspGeometry)
		{
			for (int i = 0; i < bspGeometry.Faces.Count; )
			{
				var f = bspGeometry.Faces[i];
				if (f.MaxUV0Distance > 15.5f)
				{
					bspGeometry.Faces.RemoveAt(i);
					TessalateFace(bspGeometry.Faces, f);
					continue;
				}
				++i;
			}
		}

		private void BuildFace(LevelVBWriter writer, Cb4aLevelVBSubcluster sub, ref CIwVec3 mins, ref CIwVec3 maxs, BspGeometryFace f)
		{
			
			while (f.Vertex0.UV0.X >= 8 || f.Vertex1.UV0.X >= 8 || f.Vertex2.UV0.X >= 8)
			{
				f.Vertex0.UV0.X -= 1;
				f.Vertex1.UV0.X -= 1;
				f.Vertex2.UV0.X -= 1;
			}
			while (f.Vertex0.UV0.Y >= 8 || f.Vertex1.UV0.Y >= 8 || f.Vertex2.UV0.Y >= 8)
			{
				f.Vertex0.UV0.Y -= 1;
				f.Vertex1.UV0.Y -= 1;
				f.Vertex2.UV0.Y -= 1;
			}
			if (f.Vertex0.UV0.X < -8) f.Vertex0.UV0.X = -8;
			if (f.Vertex1.UV0.X < -8) f.Vertex1.UV0.X = -8;
			if (f.Vertex2.UV0.X < -8) f.Vertex2.UV0.X = -8;
			if (f.Vertex0.UV0.Y < -8) f.Vertex0.UV0.Y = -8;
			if (f.Vertex1.UV0.Y < -8) f.Vertex1.UV0.Y = -8;
			if (f.Vertex2.UV0.Y < -8) f.Vertex2.UV0.Y = -8;

			if (f.Vertex0.Position.X < mins.x) mins.x = (int)f.Vertex0.Position.X;
			if (f.Vertex1.Position.X < mins.x) mins.x = (int)f.Vertex1.Position.X;
			if (f.Vertex2.Position.X < mins.x) mins.x = (int)f.Vertex2.Position.X;
			if (f.Vertex0.Position.Y < mins.y) mins.y = (int)f.Vertex0.Position.Y;
			if (f.Vertex1.Position.Y < mins.y) mins.y = (int)f.Vertex1.Position.Y;
			if (f.Vertex2.Position.Y < mins.y) mins.y = (int)f.Vertex2.Position.Y;
			if (f.Vertex0.Position.Z < mins.z) mins.z = (int)f.Vertex0.Position.Z;
			if (f.Vertex1.Position.Z < mins.z) mins.z = (int)f.Vertex1.Position.Z;
			if (f.Vertex2.Position.Z < mins.z) mins.z = (int)f.Vertex2.Position.Z;

			if (f.Vertex0.Position.X > maxs.x) maxs.x = (int)f.Vertex0.Position.X;
			if (f.Vertex1.Position.X > maxs.x) maxs.x = (int)f.Vertex1.Position.X;
			if (f.Vertex2.Position.X > maxs.x) maxs.x = (int)f.Vertex2.Position.X;
			if (f.Vertex0.Position.Y > maxs.y) maxs.y = (int)f.Vertex0.Position.Y;
			if (f.Vertex1.Position.Y > maxs.y) maxs.y = (int)f.Vertex1.Position.Y;
			if (f.Vertex2.Position.Y > maxs.y) maxs.y = (int)f.Vertex2.Position.Y;
			if (f.Vertex0.Position.Z > maxs.z) maxs.z = (int)f.Vertex0.Position.Z;
			if (f.Vertex1.Position.Z > maxs.z) maxs.z = (int)f.Vertex1.Position.Z;
			if (f.Vertex2.Position.Z > maxs.z) maxs.z = (int)f.Vertex2.Position.Z;

			sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex2)));
			sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex1)));
			sub.Indices.Add(writer.Write(GetLevelVBItem(f.Vertex0)));
		}

		private void TessalateFace(IList<BspGeometryFace> faces, BspGeometryFace f)
		{
			var v01 = new BspGeometryVertex() { 
				Normal = f.Vertex0.Normal, 
				Position = (f.Vertex0.Position + f.Vertex1.Position)*0.5f,
				UV0 = (f.Vertex0.UV0 + f.Vertex1.UV0) * 0.5f,
				UV1 = (f.Vertex0.UV1 + f.Vertex1.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex0.Color.A + (int)f.Vertex1.Color.A) / 2),
					(byte)(((int)f.Vertex0.Color.R + (int)f.Vertex1.Color.R) / 2),
					(byte)(((int)f.Vertex0.Color.G + (int)f.Vertex1.Color.G) / 2),
					(byte)(((int)f.Vertex0.Color.B + (int)f.Vertex1.Color.B) / 2))
			};
			var v02 = new BspGeometryVertex() { 
				Normal = f.Vertex0.Normal, 
				Position = (f.Vertex0.Position + f.Vertex2.Position)*0.5f,
				UV0 = (f.Vertex0.UV0 + f.Vertex2.UV0) * 0.5f,
				UV1 = (f.Vertex0.UV1 + f.Vertex2.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex0.Color.A + (int)f.Vertex2.Color.A) / 2),
					(byte)(((int)f.Vertex0.Color.R + (int)f.Vertex2.Color.R) / 2),
					(byte)(((int)f.Vertex0.Color.G + (int)f.Vertex2.Color.G) / 2),
					(byte)(((int)f.Vertex0.Color.B + (int)f.Vertex2.Color.B) / 2))
			};
			var v12 = new BspGeometryVertex() { 
				Normal = f.Vertex1.Normal, 
				Position = (f.Vertex1.Position + f.Vertex2.Position)*0.5f,
				UV0 = (f.Vertex1.UV0 + f.Vertex2.UV0) * 0.5f,
				UV1 = (f.Vertex1.UV1 + f.Vertex2.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex1.Color.A + (int)f.Vertex2.Color.A) / 2),
					(byte)(((int)f.Vertex1.Color.R + (int)f.Vertex2.Color.R) / 2),
					(byte)(((int)f.Vertex1.Color.G + (int)f.Vertex2.Color.G) / 2),
					(byte)(((int)f.Vertex1.Color.B + (int)f.Vertex2.Color.B) / 2))
			};
			faces.Add(new BspGeometryFace() 
			{ 
				Texture = f.Texture, Lightmap = f.Lightmap,
				Vertex0 = f.Vertex0,
				Vertex1 = v01,
				Vertex2 = v02
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = f.Vertex1,
				Vertex1 = v12,
				Vertex2 = v01
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = f.Vertex2,
				Vertex1 = v02,
				Vertex2 = v12
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = v01,
				Vertex1 = v12,
				Vertex2 = v02
			});
		}

		private LevelVBItem GetLevelVBItem(BspGeometryVertex bspGeometryVertex)
		{
			return new LevelVBItem() { Position = GetVec3(bspGeometryVertex.Position),
									   Normal = GetVec3Fixed(bspGeometryVertex.Normal),
									   UV0 = GetVec2Fixed(bspGeometryVertex.UV0),
									   UV1 = GetVec2Fixed(bspGeometryVertex.UV1),
									   Colour = GetColour(bspGeometryVertex.Color)
			};
		}

		private void WriteGeometry(BspGeometry bspGeometry, ModelWriter modelWriter)
		{
			foreach (var face in bspGeometry.Faces)
			{
				
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
				node.Front = AddTreeNode(l, bspTreeNode.Front);
				node.IsBackLeaf = bspTreeNode.Back is BspTreeLeaf;
				node.Back = AddTreeNode(l, bspTreeNode.Back);
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
		private CIwColour GetColour(System.Drawing.Color col)
		{
			return new CIwColour() { r=col.R, g=col.G,b=col.B,a=col.A };
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
