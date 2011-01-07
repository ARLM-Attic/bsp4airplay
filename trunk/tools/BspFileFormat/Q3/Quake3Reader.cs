using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;

namespace BspFileFormat.Q3
{
	class Quake3Reader : IBspReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected string entities;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadEntities(source);

			ReaderHelper.BuildEntities(entities, dest);
		}
		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.entities);
			int size = (int)(header.entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}
		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}
	}
}
