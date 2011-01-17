using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;
using System.Drawing;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q3
{
	class Quake3Reader : IBspReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected string entities;
		protected List<BspTexture> textures;
		protected List<BspTexture> lightmaps;
		protected List<plane_t> planes;
		protected List<node_t> nodes;
		protected List<leaf_t> dleaves;
		protected List<face_t> faces;
		protected List<BspTreeLeaf> leaves;
		protected List<vertex_t> vertices;
		protected List<cluster_t> clusters = new List<cluster_t>();

		protected uint[] listOfFaces;
		protected uint[] listOfVertices;
		private List<texture_t> texInfo;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadEntities(source);
			ReadTextures(source);
			ReadLightmaps(source);
			ReadPlanes(source);
			ReadNodes(source);
			ReadLeaves(source);
			ReadVisibility(source);
			listOfFaces = ReaderHelper.ReadUInt32Array(source, header.leaffaces.size, header.leaffaces.offset);
			listOfVertices = ReaderHelper.ReadUInt32Array(source, header.meshverts.size, header.meshverts.offset);
			vertices = ReaderHelper.ReadStructs<vertex_t>(source, header.vertexes.size, header.vertexes.offset + startOfTheFile, 11 * 4);
			faces = ReaderHelper.ReadStructs<face_t>(source, header.faces.size, header.faces.offset + startOfTheFile, 26*4);

			BuildLeaves();
			if (nodes != null && nodes.Count > 0)
				dest.Tree = BuildNode(nodes[0]);

			ReaderHelper.BuildEntities(entities, dest);
		}

		private void ReadLightmaps(BinaryReader source)
		{
			SeekDir(source, header.lightmaps);
			int size = (int)header.lightmaps.size/128/128/3;
			lightmaps = new List<BspTexture>(size);
			for (int i = 0; i < size; ++i)
			{
				var lightmap = new BspEmbeddedTexture();
				lightmap.Name = "lightmap"+i;
				Bitmap bitmap = new Bitmap(128,128);
				for (int y = 0; y < 128; ++y)
					for (int x = 0; x < 128; ++x)
					{
						var b = source.ReadBytes(3);
						bitmap.SetPixel(x, y, Color.FromArgb(255, b[0], b[1], b[2]));
					}
				lightmap.mipMaps = new Bitmap[] { bitmap };
				lightmaps.Add(lightmap);
			}
		}

		private void ReadVisibility(BinaryReader source)
		{
			SeekDir(source, header.visdata);
			int vectors = source.ReadInt32();
			int vecsize = source.ReadInt32();
			for (int i = 0; i < vectors; ++i)
			{
				var bits = source.ReadBytes(vecsize);
				int pos = 0;
				foreach (var b in bits)
				{

					for (int j = 0; j < 8; ++j)
					{
						if (0 != (b & (1 << j)))
						{
							clusters[i].visiblity.Add(pos);
						}
						++pos;
					}
				}
			}
		}
		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.entities);
			int size = (int)(header.entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}
		private void ReadTextures(BinaryReader source)
		{
			texInfo = ReaderHelper.ReadStructs<texture_t>(source, header.textures.size, header.textures.offset + startOfTheFile, 64+4+4);
			textures = new List<BspTexture>();
			foreach (var t in texInfo)
			{
				textures.Add(new BspTextureReference() { Name = t.name, Sky = 0 != (t.flags & texture_t.SURFACE_PORTALSKY) });
			}
		}
		private void ReadPlanes(BinaryReader source)
		{
			planes = ReaderHelper.ReadStructs<plane_t>(source, header.planes.size, header.planes.offset + startOfTheFile, 16);
		}
		private void ReadNodes(BinaryReader source)
		{
			nodes = ReaderHelper.ReadStructs<node_t>(source, header.nodes.size, header.nodes.offset + startOfTheFile, 9*4);
		}
		private void ReadLeaves(BinaryReader source)
		{
			dleaves = ReaderHelper.ReadStructs<leaf_t>(source, header.leafs.size, header.leafs.offset + startOfTheFile, 12 * 4);
			for (int i=0; i<dleaves.Count; ++i)
			{
				if (dleaves[i].cluster >= 0)
				{
					while (clusters.Count <= dleaves[i].cluster)
						clusters.Add(new cluster_t());
					clusters[dleaves[i].cluster].lists.Add(i);
				}
			}
		}

		private BspTreeNode BuildNode(node_t node)
		{
			var res = new BspTreeNode();
			res.Mins = new Vector3(node.box.mins[0], node.box.mins[1], node.box.mins[2]);
			res.Maxs = new Vector3(node.box.maxs[0], node.box.maxs[1], node.box.maxs[2]);

			res.PlaneDistance = planes[node.planenum].dist;
			res.PlaneNormal = planes[node.planenum].normal;

			if (0 != (node.front & 0x8000))
				res.Front = leaves[(ushort)~node.front];
			else
				res.Front = BuildNode(nodes[node.front]);
			if (0 != (node.back & 0x8000))
				res.Back = leaves[(ushort)~node.back];
			else
				res.Back = BuildNode(nodes[node.back]);
			return res;
		}
		private void BuildLeaves()
		{
			leaves = new List<BspTreeLeaf>();
			for (int i = 0; i < dleaves.Count; ++i)
			{
				leaves.Add(BuildLeaf(dleaves[i]));
			}
			for (int i = 0; i < dleaves.Count; ++i)
				if (dleaves[i].cluster >= 0)
				{
					Dictionary<int, bool> map = new Dictionary<int, bool>();
					foreach (var l in clusters[dleaves[i].cluster].lists)
						map[l] = true;
					foreach (var c in clusters[dleaves[i].cluster].visiblity)
						foreach (var l in clusters[c].lists)
							map[l] = true;
					foreach (var k in map.Keys)
						if (k != i)
							leaves[i].VisibleLeaves.Add(leaves[k]);
				}
		}

		private BspTreeLeaf BuildLeaf(leaf_t dleaf)
		{
			var res = new BspTreeLeaf();
			res.Mins = new Vector3(dleaf.box.mins[0], dleaf.box.mins[1], dleaf.box.mins[2]);
			res.Maxs = new Vector3(dleaf.box.maxs[0], dleaf.box.maxs[1], dleaf.box.maxs[2]);
			res.Geometry = BuildGeometry((uint)dleaf.leafface, (uint)dleaf.n_leaffaces);
			return res;
		}

		private BspGeometry BuildGeometry(uint fromFace, uint numFaces)
		{
			var res = new BspGeometry() { Faces = new List<BspGeometryFace>() };
			for (uint i = fromFace; i < fromFace + numFaces; ++i)
			{
				if (i < 0 || i >= (uint)listOfFaces.Length)
					throw new ArgumentOutOfRangeException(String.Format("{0}>={1}", i, listOfFaces.Length));
				int faceIndex = (int)listOfFaces[(int)i];
				if (faceIndex < 0 || faceIndex >= faces.Count)
				{
					throw new ArgumentOutOfRangeException(String.Format("{0}>={1}", faceIndex, faces.Count));
				}
				var face = faces[faceIndex];
				if (face.numOfVerts == 0)
					continue;

				switch (face.type)
				{
					case 1:
						BuildPolygonFace(res, face);
						break;
					case 2:
						BuildPathFace(res, face);
						break;
					case 3:
						BuildMeshFace(res, face);
						break;
					case 4:
						BuildBillboardFace(res, face);
						break;
				}
			}
			return res;
		}

		private void BuildBillboardFace(BspGeometry res, face_t face)
		{
		}

		private void BuildMeshFace(BspGeometry res, face_t face)
		{
			BuildPolygonFace(res, face);
		}

		private void BuildPathFace(BspGeometry res, face_t face)
		{
			var texture = (face.texinfo_id>=0)?textures[face.texinfo_id]:null;
			BspTexture lightMap = (face.lightmapID>=0)?lightmaps[face.lightmapID]:null;
			var width = face.sizeX;
			var height = face.sizeY;
			if (width * height != face.numOfVerts)
				throw new ApplicationException("wrong patch point count");
			for (int i = 0; i < width - 1; i += 1)
			{
				for (int j = 0; j < height - 1; j += 1)
				{
					var vert0 = BuildVertex((int)(face.vertexIndex + (i) + (j) * width), face);
					var vert1 = BuildVertex((int)(face.vertexIndex + (i + 1) + (j) * width), face);
					var vert2 = BuildVertex((int)(face.vertexIndex + (i + 1) + (j + 1) * width), face);


					var geoFace = new BspGeometryFace() { Vertex0 = vert2, Vertex1 = vert1, Vertex2 = vert0, Texture = texture, Lightmap = lightMap };
					res.Faces.Add(geoFace);

					vert0 = BuildVertex((int)(face.vertexIndex + (i) + (j) * width), face);
					vert1 = BuildVertex((int)(face.vertexIndex + (i + 1) + (j+1) * width), face);
					vert2 = BuildVertex((int)(face.vertexIndex + (i) + (j + 1) * width), face);

					geoFace = new BspGeometryFace() { Vertex0 = vert2, Vertex1 = vert1, Vertex2 = vert0, Texture = texture, Lightmap = lightMap };
					res.Faces.Add(geoFace);
				}
			}
		}

		private void BuildPolygonFace(BspGeometry res, face_t face)
		{
			var texture = (face.texinfo_id >= 0) ? textures[face.texinfo_id] : null;
			BspTexture lightMap = (face.lightmapID >= 0) ? lightmaps[face.lightmapID] : null;

			for (uint j = 0; j + 2 < face.numMeshVerts; j+=3)
			{
				var vert0 = BuildVertex((int)(face.vertexIndex+listOfVertices[face.meshVertIndex+j]), face);
				var vert1 = BuildVertex((int)(face.vertexIndex + listOfVertices[face.meshVertIndex + j+1]), face);
				var vert2 = BuildVertex((int)(face.vertexIndex + listOfVertices[face.meshVertIndex + j + 2]), face);
				
				var geoFace = new BspGeometryFace() { Vertex0 = vert0, Vertex1 = vert1, Vertex2 = vert2, Texture = texture, Lightmap = lightMap };
				res.Faces.Add(geoFace);
			}
		}

		private BspGeometryVertex BuildVertex(int p, face_t face)
		{
			BspGeometryVertex v = new BspGeometryVertex();
			var src = vertices[p];
			v.Position = src.vPosition;
			v.Normal = src.vNormal;
			v.UV0 = src.vTextureCoord;
			v.UV1 = src.vLightmapCoord;
			if (0 != (texInfo[face.texinfo_id].flags & texture_t.SURFACE_PORTALSKY))
			{
				v.UV0 = Vector2.g_Zero;
				v.UV1 = Vector2.g_Zero;
			}
			v.Color = Color.FromArgb(src.color[3], src.color[0], src.color[1], src.color[2]);
			return v;
		}
		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}
	}
}
