using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Math;
using System.IO;
using BspFileFormat.Utils;
using System.Drawing;

namespace BspFileFormat.Q1HL1
{
	public class QuakeReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected Color[] palette = q1palette.palette;
		protected List<Vector3> vertices;
		protected List<edge_t> edges;
		protected List<EmbeddedTexture> textures;
		protected List<face_t> faces;
		protected List<model_t> models;
		protected List<plane_t> planes;
		protected List<node_t> nodes;
		protected List<dleaf_t> dleaves;
		protected List<surface_t> surfaces;
		protected byte[] visilist;
		protected byte[] lightmap;
		protected ushort[] listOfFaces;
		protected short[] listOfEdges;
		private int maxUsedEdge;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;

			ReadHeader(source);
			ReadPlanes(source);
			ReadTextures(source);
			ReadTextureInfos(source);
			ReadVisilist(source);
			ReadEdges(source);
			ReadNodes(source);
			ReadLeaves(source);
			ReadVertices(source);
			ReadLightmap(source);
			ReadFaces(source);
			ReadListOfFaces(source);
			ReadListOfEdges(source);
			ReadModels(source);

			if (textures != null)
				foreach (var tex in textures)
					dest.AddTexture(tex);

			if (nodes != null && nodes.Count > 0)
			{
				var n = BuildNode(nodes[0]);
			}

			if (models != null)
				foreach (var model in models)
					dest.AddModel(BuildGeometry(model));
		}

		private BspGeometry BuildGeometry(model_t model)
		{
			var res = new BspGeometry() { Vertices = new List<BspGeometryVertex>(), Faces = new List<BspGeometryFace>() };
			//Dictionary<BspGeometryVertex,int> vertices = new Dictionary<BspGeometryVertex,int>();
			for (uint i = model.face_id; i < model.face_id + model.face_num; ++i)
			{
				var face = faces[listOfFaces[i]];
				if (face.ledge_num == 0)
					continue;

				var n = planes[face.plane_id].normal;
				var surf = surfaces[face.texinfo_id];
				var faceVertices = new short[face.ledge_num];

				for (int j = 0; j < (int)face.ledge_num; ++j)
				{
					var listOfEdgesIndex = (int)face.ledge_id +j;
					if (listOfEdgesIndex >= listOfEdges.Length)
						throw new ApplicationException(string.Format("Edge list index {0} is out of range [0..{1}]", listOfEdgesIndex, listOfEdges.Length - 1));
					var edgeIndex = listOfEdges[listOfEdgesIndex];
					if (edgeIndex >= edges.Count)
						throw new ApplicationException(string.Format("Edge index {0} is out of range [0..{1}]", edgeIndex, edges.Count - 1));
					edge_t edge;
					if (edgeIndex >= 0)
					{
						edge = edges[edgeIndex];
					}
					else
					{
						var flippedEdge = edges[-edgeIndex];
						edge = new edge_t() { vertex0 = flippedEdge.vertex1, vertex1 = flippedEdge.vertex0 };
					}
					var edgesvertex0 = edge.vertex0;
					if (edgesvertex0 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex0, vertices.Count - 1));
					var edgesvertex1 = edge.vertex1;
					if (edgesvertex1 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex1, vertices.Count - 1));
					faceVertices[j] = (short)edgesvertex0;
				}

				var vert0 = BuildVertex(vertices[faceVertices[0]], n, ref surf);
				for (int j = 1; j < faceVertices.Length-1; ++j)
				{
					BspGeometryVertex vert1 = BuildVertex(vertices[faceVertices[j]], n, ref surf);
					BspGeometryVertex vert2 = BuildVertex(vertices[faceVertices[j + 1]], n, ref surf);
					var faceIndex = res.Vertices.Count;
					res.Vertices.Add(vert0);
					res.Vertices.Add(vert1);
					res.Vertices.Add(vert2);
					var geoFace = new BspGeometryFace() { Vertex0 = faceIndex, Vertex1 = faceIndex+1, Vertex2 = faceIndex+2 };
					res.Faces.Add(geoFace);
				}
			}
			return res;
		}

		private BspGeometryVertex BuildVertex(Vector3 vector3, Vector3 n, ref surface_t surf)
		{
			var res = new BspGeometryVertex();
			res.Position = vector3;
			res.Normal = n;
			res.UV0 = new Vector2(Vector3.Dot(surf.vectorS, vector3) + surf.distS, Vector3.Dot(surf.vectorT, vector3) + surf.distT);
			res.UV1 = Vector2.Zero;
			return res;
		}
		private void ReadListOfEdges(BinaryReader source)
		{
			SeekDir(source, header.ledges);
			int size = (int)(header.ledges.size / 4);
			listOfEdges = new short[size];
			maxUsedEdge = 0;
			for (int i = 0; i < size; ++i)
			{
				listOfEdges[i] = (short)source.ReadInt32();
				if (listOfEdges[i] > maxUsedEdge)
					maxUsedEdge = listOfEdges[i];
			}
		}
		private void ReadListOfFaces(BinaryReader source)
		{
			SeekDir(source, header.lface);
			int size = (int)(header.lface.size/2);
			listOfFaces = new ushort[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadUInt16();
			}
		}

		private BspTreeNode BuildNode(node_t node)
		{
			var res = new BspTreeNode();
			if (0 != (node.front & 0x8000))
				res.Front = BuildLeaf(dleaves[(ushort)~node.front]);
			else
				res.Front = BuildNode(nodes[node.front]);
			if (0 != (node.back & 0x8000))
				res.Back = BuildLeaf(dleaves[(ushort)~node.back]);
			else
				res.Back = BuildNode(nodes[node.back]);
			return res;
		}

		private BspTreeLeaf BuildLeaf(dleaf_t dleaf)
		{
			var res = new BspTreeLeaf();
			return res;
		}

		protected virtual void ReadTextures(BinaryReader source)
		{
			if (header.miptex.size == 0)
				return;
			SeekDir(source, header.miptex);
			mipheader_t hdr = new mipheader_t(); 
			hdr.Read(source);
			textures = new List<EmbeddedTexture>(hdr.offset.Length);
			foreach (var offset in hdr.offset)
			{
				source.BaseStream.Seek(startOfTheFile + header.miptex.offset + offset, SeekOrigin.Begin);
				miptex_t miptex = new miptex_t();
				var texPos = source.BaseStream.Position;
				miptex.Read(source);
				var tex = new EmbeddedTexture() { name = miptex.name };
				if (miptex.offset1 > 0)
				{

					tex.mipMaps = new Bitmap[4];
					int x, y, w, h;

					tex.mipMaps[0] = new Bitmap((int)miptex.width, (int)miptex.height);
					source.BaseStream.Seek(texPos + miptex.offset1, SeekOrigin.Begin);
					w = (int)miptex.width;
					h = (int)miptex.height;
					for (y = 0; y < h; ++y)
						for (x = 0; x < w; ++x)
							tex.mipMaps[0].SetPixel(x, y, palette[(int)source.ReadByte()]);

					tex.mipMaps[1] = new Bitmap((int)miptex.width / 2, (int)miptex.height / 2);
					source.BaseStream.Seek(texPos + miptex.offset2, SeekOrigin.Begin);
					w /= 2;
					h /= 2;
					for (y = 0; y < h; ++y)
						for (x = 0; x < w; ++x)
							tex.mipMaps[1].SetPixel(x, y, palette[(int)source.ReadByte()]);

					tex.mipMaps[2] = new Bitmap((int)miptex.width / 4, (int)miptex.height / 4);
					source.BaseStream.Seek(texPos + miptex.offset4, SeekOrigin.Begin);
					w /= 2;
					h /= 2;
					for (y = 0; y < h; ++y)
						for (x = 0; x < w; ++x)
							tex.mipMaps[2].SetPixel(x, y, palette[(int)source.ReadByte()]);

					tex.mipMaps[3] = new Bitmap((int)miptex.width / 8, (int)miptex.height / 8);
					source.BaseStream.Seek(texPos + miptex.offset8, SeekOrigin.Begin);
					w /= 2;
					h /= 2;
					for (y = 0; y < h; ++y)
						for (x = 0; x < w; ++x)
							tex.mipMaps[3].SetPixel(x, y, palette[(int)source.ReadByte()]);
				}
				textures.Add(tex);
			}
		}
		private void ReadTextureInfos(BinaryReader source)
		{
			SeekDir(source, header.texinfo);
			if (header.texinfo.size % 40 != 0)
				throw new Exception();

			int size = (int)(header.texinfo.size / 40);

			surfaces = new List<surface_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new surface_t();
				v.Read(source);
				surfaces.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.texinfo.size + header.texinfo.offset)
				throw new Exception();
		}

		private void ReadNodes(BinaryReader source)
		{
			SeekDir(source, header.nodes);
			if (header.nodes.size % 24 != 0)
				throw new Exception();
				
			int size = (int)(header.nodes.size / 24);
			
			nodes = new List<node_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new node_t();
				v.Read(source);
				nodes.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.nodes.size + header.nodes.offset)
				throw new Exception();
		}

		private void ReadLeaves(BinaryReader source)
		{
			SeekDir(source, header.leaves);
			if (header.leaves.size % 28 != 0)
				throw new Exception();
			int size = (int)(header.leaves.size / 28);
			dleaves = new List<dleaf_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new dleaf_t();
				v.Read(source);
				dleaves.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.leaves.size + header.leaves.offset)
				throw new Exception();
		}

		private void ReadPlanes(BinaryReader source)
		{
			SeekDir(source, header.planes);
			if (header.planes.size % 20 != 0)
				throw new Exception();
			int size = (int)(header.planes.size / 20);
			planes = new List<plane_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new plane_t();
				v.Read(source);
				planes.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.planes.size + header.planes.offset)
				throw new Exception();
		}
		private void ReadVisilist(BinaryReader source)
		{
			SeekDir(source, header.visilist);
			int size = (int)(header.visilist.size);
			visilist = source.ReadBytes(size);
		}

		private void ReadLightmap(BinaryReader source)
		{
			SeekDir(source, header.lightmaps);
			int size = (int)(header.lightmaps.size);
			lightmap = source.ReadBytes(size);
		}

		private void ReadFaces(BinaryReader source)
		{
			SeekDir(source, header.faces);
			if (header.faces.size % 20 != 0)
				throw new Exception();
			int size = (int)(header.faces.size / 20);
			int maxUsedEdgeListIndex = 0;
			faces = new List<face_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new face_t();
				v.Read(source);
				if (maxUsedEdgeListIndex < v.ledge_id + v.ledge_num) maxUsedEdgeListIndex = v.ledge_id + v.ledge_num;
				faces.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.faces.size + header.faces.offset)
				throw new Exception();
		}

		private void ReadEdges(BinaryReader source)
		{
			SeekDir(source, header.edges);
			if (header.edges.size % 4 != 0)
				throw new Exception();
			int size = (int)(header.edges.size / 4);
			edges = new List<edge_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new edge_t();
				v.Read(source);
				edges.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.edges.size + header.edges.offset)
				throw new Exception();
		}

		private void ReadModels(BinaryReader source)
		{
			SeekDir(source, header.models);
			if (header.models.size % 64 != 0)
				throw new Exception();
			int size = (int)(header.models.size / 64);
			models = new List<model_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new model_t();
				v.Read(source);
				models.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.models.size + header.models.offset)
				throw new Exception();
		}

		private void ReadVertices(BinaryReader source)
		{
			SeekDir(source, header.vertices);
			if (header.vertices.size % 12 != 0)
				throw new Exception();
			int size = (int)(header.vertices.size / (12));
			vertices = new List<Vector3>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new Vector3();
				v.X = source.ReadSingle();
				v.Y = source.ReadSingle();
				v.Z = source.ReadSingle();
				vertices.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.vertices.size + header.vertices.offset)
				throw new Exception();
		}

		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}

		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
	}
}
