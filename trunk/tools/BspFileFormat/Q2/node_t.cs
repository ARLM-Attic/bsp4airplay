using BspFileFormat.BspMath;

namespace BspFileFormat.Q2
{
	public class node_t
	{
		public uint planenum;             // index of the splitting plane (in the plane array)

		public int front;       // index of the front child node or leaf
		public int back;        // index of the back child node or leaf

		public short minx;
		public short miny;
		public short minz;
		public short maxx;
		public short maxy;
		public short maxz;

		public ushort first_face;        // index of the first face (in the face array)
		public ushort num_faces;         // number of consecutive edges (in the face array)

		public void Read(System.IO.BinaryReader source)
		{
			planenum = source.ReadUInt32();
			front = source.ReadInt32();
			back = source.ReadInt32();
			minx = source.ReadInt16();
			miny = source.ReadInt16();
			minz = source.ReadInt16();
			maxx = source.ReadInt16();
			maxy = source.ReadInt16();
			maxz = source.ReadInt16();
			first_face = source.ReadUInt16();
			num_faces = source.ReadUInt16();
		}

	}
}
