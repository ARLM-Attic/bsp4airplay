using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	public struct edge_t
	{
		ushort vertex0;             // index of the start vertex, must be in [0,numvertices[
		ushort vertex1;             // index of the end vertex,  must be in [0,numvertices[

		internal void Read(System.IO.BinaryReader source)
		{
			vertex0 = source.ReadUInt16();
			vertex1 = source.ReadUInt16();
		}
	}

}