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
		protected byte[] visilist;
		protected byte[] lightmap;
		protected ushort[] listOfFaces;
		protected ushort[] listOfEdges;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;

			ReadHeader(source);
			ReadPlanes(source);
			ReadTextures(source);
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
		}
		private void ReadListOfEdges(BinaryReader source)
		{
			SeekDir(source, header.ledges);
			int size = (int)(header.ledges.size / 2);
			listOfFaces = new ushort[size];
			for (int i = 0; i < size; ++i)
			{
				listOfEdges[i] = source.ReadUInt16();
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
			faces = new List<face_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new face_t();
				v.Read(source);
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
