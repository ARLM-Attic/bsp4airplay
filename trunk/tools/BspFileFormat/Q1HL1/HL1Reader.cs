using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.Drawing;

namespace BspFileFormat.Q1HL1
{
	public class HL1Reader : QuakeReader, IBspReader
	{
		public virtual Bitmap BuildFaceLightmap(int p, int w, int h)
		{
			var b = new Bitmap(w, h);
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < h; ++x)
				{
					b.SetPixel(x, y, Color.FromArgb(lightmap[p], lightmap[p+1], lightmap[p+2]));
					p+=3;
				}
			return b;
		}
	}
}
