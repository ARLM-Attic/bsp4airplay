using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;

namespace BspFileFormat.Q2
{
	public class Quake2Reader : IBspReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected string entities;
		protected List<plane_t> planes;
		protected List<face_t> faces;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadEntities(source);
			ReadPlanes(source);
			ReadFaces(source);

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
		private void ReadFaces(BinaryReader source)
		{
			faces = ReaderHelper.ReadStructs<face_t>(source, header.faces.size, header.faces.offset + startOfTheFile, 2+2+4+2+2+4+4);
		}
		private void ReadPlanes(BinaryReader source)
		{
			planes = ReaderHelper.ReadStructs<plane_t>(source, header.planes.size, header.planes.offset + startOfTheFile, 20);
		}
		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}
	}
}
