using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using System.IO;
using System.Globalization;

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

				string mtl = @"CIwMaterial
{
	name """ + tex.name + @"""
	texture0 ""./" + name + @"""
}";

	//    colEmissive {0,0,0}
	//shadeMode GOURAUD
	//modulateMode RGB
	//blendMode MODULATE

				File.WriteAllText(Path.ChangeExtension(Path.Combine("textures", name), ".mtl"), mtl);
			}

			int index=0;
			foreach (var model in doc.Models)
			{
				string name = string.Format("q1_{0}.geo", index);

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("CIwModel");
				sb.AppendLine("{");
				sb.AppendFormat("\tname \"q1_{0}\"\n",index);
				sb.Append("\tCMesh\n");
				sb.Append("\t{\n");
				sb.AppendFormat("\t\tname \"q1_{0}\"\n", index);
				sb.Append("\t\tscale 1.0\n");
				sb.Append("\t\tCVerts\n");
				sb.Append("\t\t{\n");
				sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tnumVerts {0}\n", model.Vertices.Count);
				foreach (var v in model.Vertices)
					sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tv {{{0},{1},{2}}}\n", v.Position.X, v.Position.Y, v.Position.Z);
				sb.Append("\t\t}\n");

				sb.Append("\t\tCVertNorms\n");
				sb.Append("\t\t{\n");
				sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tnumVertNorms {0}\n", model.Vertices.Count);
				foreach (var v in model.Vertices)
					sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tvn {{{0},{1},{2}}}\n", v.Normal.X, v.Normal.Y, v.Normal.Z);
				sb.Append("\t\t}\n");

				sb.Append("\t\tCUVs\n");
				sb.Append("\t\t{\n");
				sb.Append("\t\t\tsetID 0\n");
				sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tnumUVs {0}\n", model.Vertices.Count);
				foreach (var v in model.Vertices)
					sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tuv {{{0},{1}}}\n", v.UV0.X , v.UV0.Y);
					//sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tuv {{{0},{1}}}\n", (int)(v.UV0.X * 4096), (int)(v.UV0.Y * 4096));
				sb.Append("\t\t}\n");

				sb.Append("\t\tCUVs\n");
				sb.Append("\t\t{\n");
				sb.Append("\t\t\tsetID 1\n");
				sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tnumUVs {0}\n", model.Vertices.Count);
				foreach (var v in model.Vertices)
					sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\tuv {{{0},{1}}}\n", v.UV1.X, v.UV1.Y);
				sb.Append("\t\t}\n");

				for (int mtlIndex = 0; mtlIndex < model.Materials.Count; ++mtlIndex)
				{
					int fcount = 0;
					foreach (var tr in model.Faces)
					{
						if (tr.MaterialIndex == mtlIndex)
							++fcount;
					}
					if (fcount > 0)
					{
						sb.Append("\t\tCSurface\n");
						sb.Append("\t\t{\n");
						sb.AppendFormat("\t\t\tmaterial \"{0}\"\n", model.Materials[mtlIndex].TextureName);
						sb.Append("\t\t\tCTris\n");
						sb.Append("\t\t\t{\n");
						sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\t\tnumTris {0}\n", fcount);
						foreach (var tr in model.Faces)
						{
							if (tr.MaterialIndex == mtlIndex)
							{
								sb.Append("\t\t\t\tt");
								sb.AppendFormat(CultureInfo.InvariantCulture, " {{{0},{1},{2},{2},-1}}", tr.Vertex0, tr.Vertex0, tr.Vertex0);
								sb.AppendFormat(CultureInfo.InvariantCulture, " {{{0},{1},{2},{2},-1}}", tr.Vertex1, tr.Vertex1, tr.Vertex1);
								sb.AppendFormat(CultureInfo.InvariantCulture, " {{{0},{1},{2},{2},-1}}", tr.Vertex2, tr.Vertex2, tr.Vertex2);
								sb.AppendLine();
							}
						}
						sb.Append("\t\t\t}\n");
						sb.Append("\t\t}\n");
					}
				}

				sb.Append("\t}\n");
				sb.AppendLine("}");

				File.WriteAllText(Path.Combine("models", name), sb.ToString(), Encoding.ASCII);

				++index;
			}
		}
		[Test]
		public void TestHL1()
		{
			//var doc = BspDocument.Load(@"..\data\maps\madcrabs.bsp");
		}
	}
}