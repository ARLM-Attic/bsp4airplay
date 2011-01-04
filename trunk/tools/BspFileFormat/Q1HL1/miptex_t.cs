using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	public struct miptex_t                // Mip Texture
	{
		public string name;             // Name of the texture.[16]
		public uint width;                // width of picture, must be a multiple of 8
		public uint height;               // height of picture, must be a multiple of 8
		public uint offset1;              // offset to u_char Pix[width   * height]
		public uint offset2;              // offset to u_char Pix[width/2 * height/2]
		public uint offset4;              // offset to u_char Pix[width/4 * height/4]
		public uint offset8;              // offset to u_char Pix[width/8 * height/8]

		public void Read(System.IO.BinaryReader source)
		{
			var n = source.ReadBytes(16);
			name = Encoding.ASCII.GetString(n).Trim('\0');
			width = source.ReadUInt32();
			height = source.ReadUInt32();
			offset1 = source.ReadUInt32();
			offset2 = source.ReadUInt32();
			offset4 = source.ReadUInt32();
			offset8 = source.ReadUInt32();
		}
	} ;
}
