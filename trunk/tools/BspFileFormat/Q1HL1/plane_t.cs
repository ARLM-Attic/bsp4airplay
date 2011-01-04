using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	public struct plane_t
	{
		Vector3 normal;               // Vector orthogonal to plane (Nx,Ny,Nz)
		// with Nx2+Ny2+Nz2 = 1
		float dist;               // Offset to plane, along the normal vector.
		// Distance from (0,0,0) to the plane
		int type;                // Type of plane, depending on normal vector.

		internal void Read(System.IO.BinaryReader source)
		{
			normal.X = source.ReadSingle();
			normal.Y = source.ReadSingle();
			normal.Z = source.ReadSingle();
			dist = source.ReadSingle();
			type = source.ReadInt32();
		}
	};
}
