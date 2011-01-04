using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	struct mipheader_t               // Mip texture list header
	{
		public int numtex;                 // Number of textures in Mip Texture list
		public int[] offset;         // Offset to each of the individual texture [numtex]

		internal void Read(System.IO.BinaryReader source)
		{
			numtex = source.ReadInt32();
			offset = new int[numtex];
			for (int i=0; i<numtex; ++i)
				offset[i] = source.ReadInt32();
		}
	} ;
}
