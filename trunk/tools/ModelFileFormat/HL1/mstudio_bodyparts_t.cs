using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class mstudio_bodyparts_t
	{
		public string name;
		public int nummodels;
		public int _base;
		public int modelindex; // index into models array
		public List<mstudio_model_t> Models;
		public void Read(BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			nummodels = source.ReadInt32();
			_base = source.ReadInt32();
			modelindex = source.ReadInt32();
		}
	}
	public class mstudio_bone_t
	{
		public string name;
		public int parent;
		public int flags;
		public int[] bonecontroller;
		public float[] value;
		public float[] scale;
		public void Read(BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(32)).Trim(new char[] { '\0' });
			parent = source.ReadInt32();
			flags = source.ReadInt32();
			bonecontroller = new int[]{source.ReadInt32(),source.ReadInt32(),source.ReadInt32(),
				source.ReadInt32(),source.ReadInt32(),source.ReadInt32()};
			value = new float[]{source.ReadSingle(),source.ReadSingle(),source.ReadSingle(),
				source.ReadSingle(),source.ReadSingle(),source.ReadSingle()};
			scale = new float[]{source.ReadSingle(),source.ReadSingle(),source.ReadSingle(),
				source.ReadSingle(),source.ReadSingle(),source.ReadSingle()};
		}
	}
}
