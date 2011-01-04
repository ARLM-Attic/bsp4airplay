using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BspFileFormat.Q1HL1;
using BspFileFormat.Utils;
using BspFileFormat.HL2;
using BspFileFormat.Q3;
using BspFileFormat.Q2;

namespace BspFileFormat
{
	public class BspDocument
	{
		List<EmbeddedTexture> embeddedTextures = new List<EmbeddedTexture>();
		public List<EmbeddedTexture> EmbeddedTextures
		{
			get
			{
				return embeddedTextures;
			}
		}

		public BspDocument()
		{
		}

		public static BspDocument Load(string p)
		{
			return Load(File.OpenRead(p));
		}

		private static BspDocument Load(FileStream fileStream)
		{
			using (BinaryReader r = new BinaryReader(fileStream))
			{
				return Load(r);
			}
		}

		private static BspDocument Load(BinaryReader r)
		{
			var res = new BspDocument();
			var pos = r.BaseStream.Position;
			var magic = r.ReadUInt32();
			IBspReader reader = null;
			if (magic == 0x1D)
				reader = new Quake1Reader();
			else if (magic == 0x1E)
				reader = new HL1Reader();
			else if (magic == 0x50534256)
				reader = new HL2Reader();
			else if (magic == 0x50534249)
			{
				magic = r.ReadUInt32();
				if (magic == 0x26)
					reader = new Quake2Reader();
				else if (magic == 0x2E)
					reader = new Quake3Reader();
			}
			if (reader == null)
				throw new ApplicationException("Format is not supported");
			r.BaseStream.Seek(pos, SeekOrigin.Begin);
			reader.ReadBsp(r, res);
			return res;
		}

		public void AddTexture(EmbeddedTexture tex)
		{
			embeddedTextures.Add(tex);
		}
	}
}
