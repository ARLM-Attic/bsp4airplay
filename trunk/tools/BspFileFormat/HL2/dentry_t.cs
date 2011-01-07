using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.HL2
{
	public struct dentry_t
	{
		public uint offset;
		public uint size;
		public uint version;
		public uint magic;

		internal void Read(System.IO.BinaryReader source)
		{
			offset = source.ReadUInt32();
			size = source.ReadUInt32();
			version = source.ReadUInt32();
			magic = source.ReadUInt32();
		}
	}
}
