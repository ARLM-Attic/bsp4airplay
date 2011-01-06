using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q1HL1
{
	public struct bboxshort_t
	{
		public short[] mins;
		public short[] maxs;

		internal void Read(System.IO.BinaryReader source)
		{
			mins = new short[3];
			maxs = new short[3];
			mins[0] = source.ReadInt16();
			mins[1] = source.ReadInt16();
			mins[2] = source.ReadInt16();
			maxs[0] = source.ReadInt16();
			maxs[1] = source.ReadInt16();
			maxs[2] = source.ReadInt16();
		}
	}
}
