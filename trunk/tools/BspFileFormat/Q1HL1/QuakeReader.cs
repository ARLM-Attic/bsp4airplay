using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.BspMath;
using System.IO;
using BspFileFormat.Utils;
using System.Drawing;
using System.Globalization;

namespace BspFileFormat.Q1HL1
{
	public class QuakeReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected Color[] palette = q1palette.palette;
		protected List<Vector3> vertices;
		protected List<edge_t> edges;
		protected List<BspEmbeddedTexture> textures;
		protected List<face_t> faces;
		protected List<model_t> models;
		protected List<plane_t> planes;
		protected List<node_t> nodes;
		protected List<dleaf_t> dleaves;
		protected List<surface_t> surfaces;
		protected List<BspTreeLeaf> leaves = new List<BspTreeLeaf>();

		protected Dictionary<int, int> lighmapSize = new Dictionary<int, int>();
		protected byte[] visilist;
		protected byte[] lightmap;
		protected ushort[] listOfFaces;
		protected short[] listOfEdges;
		protected string entities;
		private int maxUsedEdge;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;

			ReadHeader(source);
			ReadEntities(source);
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

			//if (textures != null)
			//    foreach (var tex in textures)
			//        dest.AddTexture(tex);

			BuildLeaves();
			if (nodes != null && nodes.Count > 0)
			{
				dest.Tree = BuildNode(nodes[0]);
			}

			BuildVisibilityList();

			if (models != null)
				foreach (var model in models)
					dest.AddModel(BuildGeometry(model));

			BuildEntities(dest);
		}

		private void BuildEntities(BspDocument dest)
		{
			var lines = entities.Split(new char[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
			BspEntity entity = null;
			foreach (var rawline in lines)
			{
				var line = rawline.Trim();
				if (line == "{")
				{
					entity = new BspEntity();
					dest.Entities.Add(entity);
					continue;
				}
				if (line == "}")
				{
					entity = null;
					continue;
				}
				int keyStartsAt = line.IndexOf('\"')+1;
				if (keyStartsAt <= 0)
					continue;
				int keyEndsAt = line.IndexOf('\"', keyStartsAt);
				int valueStartsAt = line.IndexOf('\"', keyEndsAt+1)+1;
				var key = line.Substring(keyStartsAt, keyEndsAt - keyStartsAt);
				var val = line.Substring(valueStartsAt, line.Length - 1 - valueStartsAt);
				if (key == "classname")
				{
					entity.ClassName = val;
				}
				else if (key == "origin")
				{
					var vals = val.Split(new char[]{' '});
					entity.Origin = new Vector3(
						float.Parse(vals[0], CultureInfo.InvariantCulture),
						float.Parse(vals[1], CultureInfo.InvariantCulture),
						float.Parse(vals[2], CultureInfo.InvariantCulture)
						);
				}
				else
				{
					entity.Values.Add(new KeyValuePair<string, string>(key,val));
				}
			}
		}

		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.entities);
			int size = (int)(header.entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}

		int lightmapTestCounter = 0;
		private BspGeometry BuildGeometry(model_t model)
		{
			var res = new BspGeometry() { Faces = new List<BspGeometryFace>() };

			for (uint i = model.face_id; i < model.face_id + model.face_num; ++i)
			{
				var face = faces[listOfFaces[i]];
				if (face.ledge_num == 0)
					continue;
				plane_t plane = planes[face.plane_id];
				var surf = surfaces[face.texinfo_id];
				var faceVertices = new BspGeometryVertex[face.ledge_num];

				Vector2 minUV0 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 minUV1 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 maxUV1 = new Vector2(float.MinValue, float.MinValue);
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
					BspGeometryVertex vertex = BuildVertex(vertices[(short)edgesvertex0], plane, ref surf);
                    faceVertices[j] = vertex;
					if (minUV0.X > vertex.UV0.X)
						minUV0.X = vertex.UV0.X;
					if (minUV0.Y > vertex.UV0.Y)
						minUV0.Y = vertex.UV0.Y;
					if (minUV1.X > vertex.UV1.X)
						minUV1.X = vertex.UV1.X;
					if (minUV1.Y > vertex.UV1.Y)
						minUV1.Y = vertex.UV1.Y;
					if (maxUV1.X < vertex.UV1.X)
						maxUV1.X = vertex.UV1.X;
					if (maxUV1.Y < vertex.UV1.Y)
						maxUV1.Y = vertex.UV1.Y;
					
				}
				minUV0.X = (float)System.Math.Floor(minUV0.X);
				minUV0.Y = (float)System.Math.Floor(minUV0.Y);

				minUV1.X = (float)System.Math.Floor(minUV1.X);
				minUV1.Y = (float)System.Math.Floor(minUV1.Y);
				maxUV1.X = (float)System.Math.Ceiling(maxUV1.X);
				maxUV1.Y = (float)System.Math.Ceiling(maxUV1.Y);
				var sizeLightmap = maxUV1 - minUV1 + new Vector2(1, 1);
				for (int j = 0; j < (int)face.ledge_num; ++j)
				{
					faceVertices[j].UV0 = faceVertices[j].UV0 - minUV0;
					faceVertices[j].UV1.X = (faceVertices[j].UV1.X - minUV1.X)/sizeLightmap.X;
					faceVertices[j].UV1.Y = (faceVertices[j].UV1.Y - minUV1.Y) / sizeLightmap.Y;
				}
				BspTexture lightMap = null;
				if (face.lightmap != -1)
				{
					var size = lighmapSize[face.lightmap];
					var size2 = (sizeLightmap.X) * (sizeLightmap.Y);
					lightMap = new BspEmbeddedTexture()
					{
						mipMaps = new Bitmap[] { BuildFaceLightmap(face.lightmap, (int)sizeLightmap.X, (int)sizeLightmap.Y) },
						Width = (int)sizeLightmap.X,
						Height = (int)sizeLightmap.Y
					};
				}
				
				var vert0 = faceVertices[0];
				for (int j = 1; j < faceVertices.Length-1; ++j)
				{
					BspGeometryVertex vert1 = faceVertices[j];
					BspGeometryVertex vert2 = faceVertices[j + 1];
					var geoFace = new BspGeometryFace() { Vertex0 = vert0, Vertex1 = vert1, Vertex2 = vert2, Texture = textures[(int)surfaces[face.texinfo_id].texture_id], Lightmap = lightMap };
					res.Faces.Add(geoFace);
				}
			}
			return res;
		}

		public virtual Bitmap BuildFaceLightmap(int p, int w, int h)
		{
			var b = new Bitmap(w, h);
			for (int y=0; y<h;++y)
				for (int x = 0; x < w; ++x)
				{
					b.SetPixel(x, y, Color.FromArgb(lightmap[p], lightmap[p], lightmap[p]));
					++p;
				}
			return b;
		}

		private BspGeometryVertex BuildVertex(Vector3 vector3, plane_t plane, ref surface_t surf)
		{
			var res = new BspGeometryVertex();
			res.Position = vector3;
			res.Normal = plane.normal;
			//res.UV0 = new Vector2(vector3.X, vector3.Y);
			res.UV0 = new Vector2(Vector3.Dot(surf.vectorS, vector3) + surf.distS, Vector3.Dot(surf.vectorT, vector3) + surf.distT);
			res.UV1 = new Vector2(res.UV0.X / 16.0f, res.UV0.Y/16.0f);
			/*
			switch (plane.type)
			{
				case 0: //PLANE_X
				case 3: //PLANE_ANYX
					res.UV1 = new Vector2(vector3.Y / 16.0f, vector3.Z / 16.0f);
					break;
				case 1:
				case 4:
					res.UV1 = new Vector2(vector3.X / 16.0f, vector3.Z / 16.0f);
					break;
				case 2:
				case 5:
					res.UV1 = new Vector2(vector3.X / 16.0f, vector3.Y / 16.0f);
					break;
				default:
					throw new ApplicationException("Unknown plane type");
			}
			*/
			res.UV0 = new Vector2(res.UV0.X / (float)textures[(int)surf.texture_id].Width, res.UV0.Y / (float)textures[(int)surf.texture_id].Height);
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
		private void BuildLeaves()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				leaves.Add(BuildLeaf(dleaves[i]));
			}
		}
		private void BuildVisibilityList()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				BuildVisibilityList(leaves[i], dleaves[i].vislist);
			}
		}

		private void BuildVisibilityList(BspTreeLeaf bspTreeLeaf, int v)
		{
			if (v < 0)
				return;
			// Suppose Leaf is the leaf the player is in.
			for (int L = 1; L < dleaves.Count && v<visilist.Length; v++)
			{
				if (visilist[v] == 0)           // value 0, leaves invisible
				{
					L += 8 * visilist[v + 1];    // skip some leaves
					v++;
				}
				else                          // tag 8 leaves, if needed
				{                           // examine bits right to left
					for (byte bit = 1; bit != 0 && L < dleaves.Count; bit = (byte)(bit << 1), ++L)
					{
						if (0 != (visilist[v] & bit))
						{
							if (L >= leaves.Count)
								throw new ApplicationException(string.Format("leaf index {0} is out of {1}",L,leaves.Count));
							bspTreeLeaf.VisibleLeaves.Add(leaves[L]);
						}
					}
				}
			}
		}

		private BspTreeNode BuildNode(node_t node)
		{
			var res = new BspTreeNode();
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
			textures = new List<BspEmbeddedTexture>(hdr.offset.Length);
			foreach (var offset in hdr.offset)
			{
				source.BaseStream.Seek(startOfTheFile + header.miptex.offset + offset, SeekOrigin.Begin);
				miptex_t miptex = new miptex_t();
				var texPos = source.BaseStream.Position;
				miptex.Read(source);
				var tex = new BspEmbeddedTexture() { Name = miptex.name, Width = (int)miptex.width, Height = (int)miptex.height };
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
			List<int> lighmapBorders = new List<int>();
			
			faces = new List<face_t>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new face_t();
				v.Read(source);
				if (v.lightmap != -1)
					lighmapBorders.Add(v.lightmap);
				if (maxUsedEdgeListIndex < v.ledge_id + v.ledge_num) 
					maxUsedEdgeListIndex = v.ledge_id + v.ledge_num;
				faces.Add(v);
			}
			lighmapBorders.Sort();
			for (int i = 0; i < lighmapBorders.Count - 1; ++i)
				lighmapSize[lighmapBorders[i]] = lighmapBorders[i + 1] - lighmapBorders[i];
			lighmapSize[lighmapBorders[lighmapBorders.Count - 1]] = lightmap.Length - lighmapBorders[lighmapBorders.Count-1];
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
