using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using BspFileFormat.BspMath;

namespace BspFileFormat
{
	public class BspGeometryFace
	{
		public BspGeometryVertex Vertex0;
		public BspGeometryVertex Vertex1;
		public BspGeometryVertex Vertex2;
		public BspTexture Texture;
		public BspTexture Lightmap;
	}
}
