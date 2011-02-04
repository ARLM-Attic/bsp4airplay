using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

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
	public class mstudio_seq_desc_t
	{
		public string label; // sequence label
        public float fps; // frames per second
        public int flags; // looping/non-looping flags
        public int activity;
        public int actweight;
        public int numevents;
        public int eventindex;
        public int numframes; // number of frames per sequence
        public int numpivots; // number of foot pivots
        public int pivotindex;
        public int motiontype;
        public int motionbone;
        public Vector3 linearmovement;
        public int automoveposindex;
        public int automoveangleindex;
        public Vector3 bbmin; // per sequence bounding box
        public Vector3 bbmax;
        public int numblends;
        public int animindex; /* mstudioanim_t pointer relative to start of
                                          sequence group data */
        // [blend][bone][X, Y, Z, XR, YR, ZR]
        public int[] blendtype = new int[2]; // X, Y, Z, XR, YR, ZR
        public float[] blendstart = new float[2]; // starting value
		public float[] blendend = new float[2]; // ending value
        public int blendparent;
        public int seqgroup; // sequence group for demand loading
        public int entrynode; // transition node at entry
        public int exitnode; // transition node at exit
        public int nodeflags; // transition rules
        public int nextseq; // auto advancing sequences

		public void Read(BinaryReader source)
		{
			label = Encoding.ASCII.GetString(source.ReadBytes(32)).Trim(new char[] { '\0' });

			fps = source.ReadSingle(); // frames per second
			flags = source.ReadInt32(); // looping/non-looping flags
			activity = source.ReadInt32();
			actweight = source.ReadInt32();
			numevents = source.ReadInt32();
			eventindex = source.ReadInt32();
			numframes = source.ReadInt32(); // number of frames per sequence
			numpivots = source.ReadInt32(); // number of foot pivots
			pivotindex = source.ReadInt32();
			motiontype = source.ReadInt32();
			motionbone = source.ReadInt32();
			linearmovement = new Vector3(source.ReadSingle(),source.ReadSingle(),source.ReadSingle());
			automoveposindex = source.ReadInt32();
			automoveangleindex = source.ReadInt32();
			bbmin = new Vector3(source.ReadSingle(), source.ReadSingle(), source.ReadSingle()); // per sequence bounding box
			bbmax = new Vector3(source.ReadSingle(), source.ReadSingle(), source.ReadSingle());
			numblends = source.ReadInt32();
			animindex = source.ReadInt32(); /* mstudioanim_t pointer relative to start of
									  sequence group data */
			blendtype[0] = source.ReadInt32(); // X, Y, Z, XR, YR, ZR
			blendtype[1] = source.ReadInt32();
			blendstart[0] = source.ReadSingle(); // starting value
			blendstart[1] = source.ReadSingle();
			blendend[0] = source.ReadSingle(); // ending value
			blendend[1] = source.ReadSingle();
			blendparent = source.ReadInt32();
			seqgroup = source.ReadInt32(); // sequence group for demand loading
			entrynode = source.ReadInt32(); // transition node at entry
			exitnode = source.ReadInt32(); // transition node at exit
			nodeflags = source.ReadInt32(); // transition rules
			nextseq = source.ReadInt32(); // auto advancing sequences
		}
	}
	public class mstudio_seqgroup_t
	{
		public string label;
		public string name;
		int cache;
		public int data;
		public void Read(BinaryReader source)
		{
			label = Encoding.ASCII.GetString(source.ReadBytes(32)).Trim(new char[] { '\0' });
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			cache = source.ReadInt32();
			data = source.ReadInt32();
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

		internal Vector3 Transform(Vector3 pos)
		{
			return new Vector3(pos.X + value[0], pos.Y + value[1], pos.Z + value[2]);
		}
	}
}
