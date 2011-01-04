using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	public struct model_t
	{
		boundbox_t bound;            // The bounding box of the Model
		Vector3 origin;               // origin of model, usually (0,0,0)
		uint node_id0;               // index of first BSP node
		uint node_id1;               // index of the first Clip node
		uint node_id2;               // index of the second Clip node
		uint node_id3;               // usually zero
		uint numleafs;               // number of BSP leaves
		uint face_id;                // index of Faces
		uint face_num;               // number of Faces

		internal void Read(System.IO.BinaryReader source)
		{
			bound.Read(source);
			origin.X = source.ReadSingle();
			origin.Y = source.ReadSingle();
			origin.Z = source.ReadSingle();
			node_id0 = source.ReadUInt32();
			node_id1 = source.ReadUInt32();
			node_id2 = source.ReadUInt32();
			node_id3 = source.ReadUInt32();
			numleafs = source.ReadUInt32();
			face_id = source.ReadUInt32();
			face_num = source.ReadUInt32();
		}
	}
}
