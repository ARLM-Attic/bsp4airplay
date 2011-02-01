using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;
using System.Drawing;

namespace ModelFileFormat.HL1
{
	/// <summary>
	/// Sample reader: http://www.google.com/codesearch/p?hl=ru#Dss3okGmsW8/plugins/modules/model/halflife/HalfLife.cpp&q=mdl%20halflife&sa=N&cd=6&ct=rc
	/// </summary>
	public class MdlReader: IModelReader
	{
		header_t header;
		private long startOfTheFile;
		private List<mstudio_texture_t> textures;
		private List<Bitmap> textureImages;
		private List<mstudio_bodyparts_t> bodyParts;
		#region IModelReader Members

		public void ReadModel(BinaryReader source, ModelDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadTextures(source);
			ReadBodyParts(source);
			//dest.Name = header.name;
		}

		private void ReadBodyParts(BinaryReader source)
		{
			if (header.numbodyparts == 0)
				return;
			bodyParts = ReaderHelper.ReadStructs<mstudio_bodyparts_t>(source, (uint)header.numbodyparts * 76, header.bodypartindex + startOfTheFile, 76);
		}

		private void ReadTextures(BinaryReader source)
		{
			if (header.textureindex == 0) 
				return;

			textures = ReaderHelper.ReadStructs<mstudio_texture_t>(source, (uint)header.numtextures * 80, header.textureindex + startOfTheFile, 80);

			foreach (var t in textures)
			{
			}
		}

		#endregion

		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
	}
}