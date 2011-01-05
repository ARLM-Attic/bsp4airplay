using System.Collections.Generic;

namespace AirplaySDKFileFormats.Model
{
	public class CMesh
	{
		public string Name;
		public float Scale;
		public CVerts Verts;
		public CVertNorms VertNorms;
		public IList<CUVs> UVs;
		public IList<CSurface> Surfaces;
	}
}
