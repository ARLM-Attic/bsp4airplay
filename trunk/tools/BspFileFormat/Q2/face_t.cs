using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;

namespace BspFileFormat.Q2
{
		public class face_t
			{
				ushort   plane;             // index of the plane the face is parallel to
				ushort   plane_side;        // set if the normal is parallel to the plane normal

				uint   first_edge;        // index of the first edge (in the face edge array)
				ushort   num_edges;         // number of consecutive edges (in the face edge array)
			
				ushort   texture_info;      // index of the texture info structure	

				byte[] lightmap_syles; // styles (bit flags) for the lightmaps
				uint   lightmap_offset;   // offset of the lightmap (in bytes) in the lightmap lump

				public void Read(System.IO.BinaryReader source)
				{
					plane = source.ReadUInt16();
					plane_side = source.ReadUInt16();
					first_edge = source.ReadUInt32();
					num_edges = source.ReadUInt16();
					texture_info = source.ReadUInt16();
					lightmap_syles = source.ReadBytes(4);
					lightmap_offset = source.ReadUInt32();
				}
			};
}
