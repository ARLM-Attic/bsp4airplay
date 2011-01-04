using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using System.IO;

namespace Tests
{
	[TestFixture]
	public class Q1Tests
	{
		[Test]
		public void TestQ1()
		{
			var doc = BspDocument.Load(@"..\data\maps\sg0503.bsp");
			foreach (var tex in doc.EmbeddedTextures)
			{
				string name = tex.name + ".png";
				foreach (var c in Path.GetInvalidFileNameChars())
					name = name.Replace(c, '_');
				tex.mipMaps[0].Save(Path.Combine("textures", name));
			}
		}
		[Test]
		public void TestHL1()
		{
			var doc = BspDocument.Load(@"..\data\maps\madcrabs.bsp");
		}
	}
}