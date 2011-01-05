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
	}
}
