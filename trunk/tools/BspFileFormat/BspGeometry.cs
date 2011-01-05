using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using BspFileFormat.Math;

namespace BspFileFormat
{
	public class BspGeometry
	{
		public IList<BspGeometryVertex> Vertices;
		public IList<BspGeometryFace> Faces;
		public IList<BspGeometryMaterial> Materials;
	}
}
