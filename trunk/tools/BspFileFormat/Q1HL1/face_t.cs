using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	public struct face_t
	{
		ushort plane_id;            // The plane in which the face lies
		//           must be in [0,numplanes[ 
		ushort side;                // 0 if in front of the plane, 1 if behind the plane
		int ledge_id;               // first edge in the List of edges
		//           must be in [0,numledges[
		ushort ledge_num;           // number of edges in the List of edges
		ushort texinfo_id;          // index of the Texture info the face is part of
		//           must be in [0,numtexinfos[ 
		byte typelight;            // type of lighting, for the face
		byte baselight;            // from 0xFF (dark) to 0 (bright)
		byte[] light;             // two additional light models  [2]
		int lightmap;               // Pointer inside the general light map, or -1
		// this define the start of the face light map

		internal void Read(System.IO.BinaryReader source)
		{
			plane_id = source.ReadUInt16();
			side = source.ReadUInt16();
			ledge_id = source.ReadInt32();
			ledge_num = source.ReadUInt16();
			texinfo_id = source.ReadUInt16();

			typelight = source.ReadByte();
			baselight = source.ReadByte();
			light = source.ReadBytes(2);

			lightmap = source.ReadInt32();
		}
	} ;
}
