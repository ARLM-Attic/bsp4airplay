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
		private List<Bitmap> textureImages = new List<Bitmap>();
		private List<ModelTexture> modelTextures = new List<ModelTexture>();
		private List<mstudio_bodyparts_t> bodyParts;
		private List<mstudio_bone_t> bones;
		private List<ModelBone> modelBones = new List<ModelBone>();
		#region IModelReader Members

		public void ReadModel(BinaryReader source, ModelDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadTextures(source);
			ReadBones(source);
			ReadBodyParts(source);
			//dest.Name = header.name;

			foreach (var bp in bodyParts)
				dest.Meshes.Add(BuildMesh(bp));
		}

		private void ReadBones(BinaryReader source)
		{
			bones = ReaderHelper.ReadStructs<mstudio_bone_t>(source, (uint)header.numbones * 112, header.boneindex + startOfTheFile, 112);
			foreach (var b in bones)
			{
				var bone = new ModelBone();
				modelBones.Add(bone);
			}
		}

		private ModelMesh BuildMesh(mstudio_bodyparts_t bp)
		{
			var mesh = new ModelMesh();
			BuildMesh(bp, mesh);
			return mesh;
		}

		private void BuildMesh(mstudio_bodyparts_t bp, ModelMesh mesh)
		{
			foreach (var m in bp.Models)
				BuildMesh(m, mesh);
		}

		private void BuildMesh(mstudio_model_t mdl, ModelMesh mesh)
		{
			foreach (var m in mdl.Meshes)
				BuildMesh(m, mesh);
		}

		private void BuildMesh(mstudio_mesh_t src, ModelMesh mesh)
		{
			foreach (var face in src.Faces)
				mesh.Faces.Add(face);
		}
		private void ReadBodyParts(BinaryReader source)
		{
			if (header.numbodyparts == 0)
				return;
			bodyParts = ReaderHelper.ReadStructs<mstudio_bodyparts_t>(source, (uint)header.numbodyparts * 76, header.bodypartindex + startOfTheFile, 76);

			foreach (var bp in bodyParts)
			{
				bp.Models = ReaderHelper.ReadStructs<mstudio_model_t>(source, (uint)bp.nummodels * 112, 
					bp.modelindex + startOfTheFile, 112);

				foreach (var mdl in bp.Models)
				{
					if (mdl.vertinfoindex != 0)
					{
						source.BaseStream.Seek(mdl.vertinfoindex + startOfTheFile, SeekOrigin.Begin);
						mdl.Weights = source.ReadBytes(mdl.numverts);
					}
					if (mdl.vertindex != 0)
					{
						mdl.Vertices = new Vector3[mdl.numverts];
						source.BaseStream.Seek(mdl.vertindex + startOfTheFile, SeekOrigin.Begin);
						for (int i = 0; i < mdl.numverts; ++i)
						{
							mdl.Vertices[i].X = source.ReadSingle()*3;
							mdl.Vertices[i].Y = source.ReadSingle() * 3;
							mdl.Vertices[i].Z = source.ReadSingle() * 3;
						}
					}
					if (mdl.numnorms != 0)
					{
						mdl.Normals = new Vector3[mdl.numnorms];
						source.BaseStream.Seek(mdl.normindex + startOfTheFile, SeekOrigin.Begin);
						for (int i = 0; i < mdl.numnorms; ++i)
						{
							mdl.Normals[i].X = source.ReadSingle();
							mdl.Normals[i].Y = source.ReadSingle();
							mdl.Normals[i].Z = source.ReadSingle();
						}
					}
					mdl.Meshes = ReaderHelper.ReadStructs<mstudio_mesh_t>(source, (uint)mdl.nummesh * 20,
						mdl.meshindex + startOfTheFile, 20);

					foreach (var mesh in mdl.Meshes)
					{
						source.BaseStream.Seek(mesh.triindex + startOfTheFile, SeekOrigin.Begin);
						int countTris=0;
						for (; countTris < mesh.numtris; )
						{
							int numVertices = source.ReadInt16();
							if (numVertices == 0)
								break;
							if (numVertices < 0)
							{
								countTris += -numVertices -2;
								ReadTrianglesFan(source, mesh, mdl, -numVertices);
							}
							else
							{
								countTris += numVertices-2;
								ReadTrianglesStrip(source, mesh, mdl, numVertices);
							}
							
						}

					}
				}
			}
		}

		private void ReadTrianglesStrip(BinaryReader source, mstudio_mesh_t mesh, mstudio_model_t mdl, int numVertices)
		{
			
			mesh_vertex_t v0 = new mesh_vertex_t();
			v0.Read(source);
			mesh_vertex_t v1 = new mesh_vertex_t();
			v1.Read(source);
			for (int i = 2; i < numVertices; ++i)
			{
				mesh_vertex_t v2 = new mesh_vertex_t();
				v2.Read(source);

				BuildTriangle(v0, v1, v2, mesh, mdl);

				//v0 = v1;
				//v1 = v2;
				if (0 == (i & 1))
					v0 = v2;
				else
					v1 = v2;
			}
		}

		private void ReadTrianglesFan(BinaryReader source, mstudio_mesh_t mesh, mstudio_model_t mdl, int numVertices)
		{
			mesh_vertex_t v0 = new mesh_vertex_t();
			v0.Read(source);
			mesh_vertex_t v1 = new mesh_vertex_t();
			v1.Read(source);
			for (int i = 2; i < numVertices; ++i)
			{
				mesh_vertex_t v2 = new mesh_vertex_t();
				v2.Read(source);

				BuildTriangle(v0, v1, v2, mesh, mdl);
				
				v1 = v2;
			}
		}

		private void BuildTriangle(mesh_vertex_t v0, mesh_vertex_t v1, mesh_vertex_t v2, mstudio_mesh_t mesh, mstudio_model_t mdl)
		{
			var textureId = mesh.skinref;
			if ((v0.v == v1.v) || (v0.v == v2.v) || (v1.v == v2.v))
				return;
			mesh.Faces.Add(new ModelFace() {
				Texture = modelTextures[textureId],
				Vertex2 = BuildVertex(v0, mdl, textureId),
				Vertex1 = BuildVertex(v1, mdl, textureId),
				Vertex0 = BuildVertex(v2, mdl, textureId)
			});
		}

		private ModelVertex BuildVertex(mesh_vertex_t v0, mstudio_model_t mdl, int textureId)
		{
			var tex = this.textures[textureId];
			return new ModelVertex() { Position = mdl.Vertices[v0.v], Normal = mdl.Normals[v0.n], UV0 = new Vector2((float)v0.s / (float)tex.width, 
				(float)v0.t / (float)tex.height),
									   Bones = new ModelBoneWeight[] { new ModelBoneWeight() { Weight = 1.0f, Bone = modelBones[mdl.Weights[v0.v]] } }
			};
		}

		private void ReadTextures(BinaryReader source)
		{
			if (header.textureindex == 0) 
				return;

			textures = ReaderHelper.ReadStructs<mstudio_texture_t>(source, (uint)header.numtextures * 80, header.textureindex + startOfTheFile, 80);
			foreach (var t in textures)
			{
				string name = t.name;
				if (name.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase))
					name = name.Substring(0, name.Length - 4);
				if (t.index != 0)
				{
					source.BaseStream.Seek(t.index + startOfTheFile, SeekOrigin.Begin);

					var bytes = source.ReadBytes(t.width * t.height);
					var pal = source.ReadBytes(768);
					var bmp = new Bitmap(t.width, t.height);
					textureImages.Add(bmp);
					for (int y = 0; y < t.height; ++y)
						for (int x = 0; x < t.width; ++x)
						{
							var col = 3 * bytes[t.width * y + x];
							bmp.SetPixel(x, y, Color.FromArgb(pal[col], pal[col + 1], pal[col + 2]));
						}
					modelTextures.Add(new ModelEmbeddedTexture(name, bmp));
				}
				else
				{
					modelTextures.Add(new ModelTextureReference(name));
				}
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