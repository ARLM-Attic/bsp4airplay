using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class ModelWriter
	{
		Dictionary<CIwVec3, int> positions = new Dictionary<CIwVec3,int>();
		Dictionary<CIwVec3, int> normals = new Dictionary<CIwVec3,int>();
		Dictionary<CIwVec2, int> uv0s = new Dictionary<CIwVec2,int>();
		Dictionary<CIwVec2, int> uv1s = new Dictionary<CIwVec2,int>();
		Dictionary<CIwColour, int> colors = new Dictionary<CIwColour,int>();
		Dictionary<string, int> surfaces = new Dictionary<string, int>();
		CMesh targetMesh;

		public ModelWriter(CMesh targetMesh)
		{
			this.targetMesh = targetMesh;
		}

		

		public int GetSurfaceIndex(string material)
		{
			int i;
			if (surfaces.TryGetValue(material, out i))
				return i;
			i = targetMesh.Surfaces.Count;
			targetMesh.Surfaces.Add(new CSurface(){Material = material});
			surfaces[material] = i;
			return i;
		}

		public int GetPositionIndex(CIwVec3 v)
		{
			int i;
			if (positions.TryGetValue(v, out i))
				return i;
			i = targetMesh.Verts.Positions.Count;
			targetMesh.Verts.Positions.Add(v);
			positions[v] = i;
			return i;
		}

		public int GetNormalIndex(CIwVec3 v)
		{
			int i;
			if (normals.TryGetValue(v, out i))
				return i;
			i = targetMesh.VertNorms.Normals.Count;
			targetMesh.VertNorms.Normals.Add(v);
			normals[v] = i;
			return i;
		}
		public int GetColorIndex(CIwColour v)
		{
			int i;
			if (colors.TryGetValue(v, out i))
				return i;
			i = targetMesh.VertCols.Colours.Count;
			targetMesh.VertCols.Colours.Add(v);
			colors[v] = i;
			return i;
		}
		public int GetUV0Index(CIwVec2 v)
		{
			int i;
			if (uv0s.TryGetValue(v, out i))
				return i;
			while (targetMesh.UVs.Count <= 0)
				targetMesh.UVs.Add(new CUVs() { SetID = targetMesh.UVs.Count });
			i = targetMesh.UVs[0].UVs.Count;
			targetMesh.UVs[0].UVs.Add(v);
			uv0s[v] = i;
			return i;
		}
		public int GetUV1Index(CIwVec2 v)
		{
			int i;
			if (uv1s.TryGetValue(v, out i))
				return i;
			while (targetMesh.UVs.Count <= 1)
				targetMesh.UVs.Add(new CUVs() { SetID = targetMesh.UVs.Count });
			i = targetMesh.UVs[1].UVs.Count;
			targetMesh.UVs[1].UVs.Add(v);
			uv1s[v] = i;
			return i;
		}

		public CTrisVertex GetVertex(CIwVec3 p, CIwVec3 n, CIwVec2 uv0, CIwVec2 uv1, CIwColour col)
		{
			return new CTrisVertex(GetPositionIndex(p), GetNormalIndex(n), GetUV0Index(uv0), GetUV1Index(uv1), GetColorIndex(col));
		}

		public void AddTriangle(int surface, CTrisVertex v0, CTrisVertex v1, CTrisVertex v2)
		{
			var t = new CTrisElement() { Vertex0 = v0, Vertex1 = v1, Vertex2 = v2 };
			targetMesh.Surfaces[surface].Triangles.Elements.Add(t);
		}
	}
	public class LevelVBWriter
	{
		Dictionary<LevelVBItem, int> map = new Dictionary<LevelVBItem, int>();
		
		private Cb4aLevel level;

		public LevelVBWriter(Cb4aLevel level)
		{
			// TODO: Complete member initialization
			this.level = level;
		}

		public int Write(LevelVBItem levelVBItem)
		{
			int index;
			if (!map.TryGetValue(levelVBItem, out index))
			{
				index = level.vb.Count;
				level.vb.Add(levelVBItem);
			}
			return index;
		}
	}
	public class Cb4aLevelVBSubcluster: CIwParseable
	{
		public List<int> Indices = new List<int>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_indices", Indices.Count);
			foreach (var i in Indices)
				writer.WriteKeyVal("t",i);
		}
	}
	public class Cb4aLevelVBCluster: CIwParseable
	{
		public List<Cb4aLevelVBSubcluster> Subclusters = new List<Cb4aLevelVBSubcluster>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_subclusters", Subclusters.Count);
			foreach (var i in Subclusters)
				i.WrtieToStream(writer);
		}
	}
	public struct LevelVBItem
	{
		public CIwVec3 Position;
		public CIwVec3 Normal;
		public CIwVec2 UV0;
		public CIwVec2 UV1;
		public CIwColour Colour;

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ UV0.GetHashCode() ^ UV1.GetHashCode() ^ Colour.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is LevelVBItem))
				return false;

			return this.Equals((LevelVBItem)obj);
		}
		public bool Equals(LevelVBItem other)
		{
			return
				Position == other.Position &&
				Normal == other.Normal &&
				UV0 == other.UV0 &&
				UV1 == other.UV1 &&
				Colour == other.Colour
				;
		}
		public static bool operator ==(LevelVBItem left, LevelVBItem right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(LevelVBItem left, LevelVBItem right)
		{
			return !left.Equals(right);
		}
		//public override string ToString()
		//{
		//    return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2}}}", x, y, z);
		//}

		internal void WrtieToStream(CTextWriter writer)
		{
			writer.WriteVec3("v", Position);
			writer.WriteVec3("vn", Normal);
			writer.WriteVec2("uv0", UV0);
			writer.WriteVec2("uv1", UV1);
			writer.WriteColour("col", Colour);
		}
	}
}
