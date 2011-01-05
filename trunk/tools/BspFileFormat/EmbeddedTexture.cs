using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BspFileFormat
{
	public class EmbeddedTexture
	{
		public string name;
		public Bitmap[] mipMaps;
		public int Width;
		public int Height;
	}
}
