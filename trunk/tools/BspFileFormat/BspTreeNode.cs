using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BspFileFormat
{
	public class BspTreeNode : BspTreeElement
	{
		public BspTreeElement Front { get; set; }
		public BspTreeElement Back { get; set; }
	}
}
