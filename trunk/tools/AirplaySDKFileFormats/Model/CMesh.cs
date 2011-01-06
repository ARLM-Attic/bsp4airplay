using System.Collections.Generic;

namespace AirplaySDKFileFormats.Model
{
	/// <summary>
	/// CMesh
	/// Airplay SDK implements this through ParseMesh helper class
	/// </summary>
	public class CMesh : CIwParseable
	{
		public string Name;
		public float Scale;
		public CVerts Verts;
		public CVertNorms VertNorms;
		public IList<CUVs> UVs;
		public IList<CSurface> Surfaces;
	}
}
