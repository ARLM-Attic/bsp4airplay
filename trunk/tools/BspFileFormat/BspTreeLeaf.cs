using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BspFileFormat
{
	public class BspTreeLeaf : BspTreeElement
	{
		public List<BspTreeLeaf> VisibleLeaves = new List<BspTreeLeaf>();
		public BspGeometry Geometry;
	}
}
