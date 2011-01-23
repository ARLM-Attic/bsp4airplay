using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace BspFileFormat.BspMath
{
	public struct Plane
	{
		public Vector3 Normal;
		public float Distance;
	}
}
