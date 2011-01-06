using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using BspFileFormat.BspMath;

namespace BspFileFormat
{
	public class BspTreeNode : BspTreeElement
	{
		public Vector3 PlaneNormal;
		public float PlaneDistance;

		public BspTreeElement Front { get; set; }
		public BspTreeElement Back { get; set; }
	}
	public class BspEntity
	{
		public string ClassName;
		public Vector3 Origin = Vector3.Zero;
		public List<KeyValuePair<string, string>> Values = new List<KeyValuePair<string, string>>();
	}
}
