using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using BspFileFormat.Math;

namespace BspFileFormat
{
	public class BspGeometryVertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV0;
		public Vector2 UV1;

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ UV0.GetHashCode() ^ UV1.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BspGeometryVertex))
				return false;

			return this.Equals((BspGeometryVertex)obj);
		}

		public bool Equals(BspGeometryVertex other)
		{
			return
				Position == other.Position &&
				Normal == other.Normal &&
				UV0 == other.UV0 &&
				UV1 == other.UV1;
		}
	}
}
