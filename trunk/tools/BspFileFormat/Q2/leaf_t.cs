using BspFileFormat.BspMath;

namespace BspFileFormat.Q2
{
	public class leaf_t
	{
		public uint brush_or;          // ?

		public short cluster;           // -1 for cluster indicates no visibility information
		public ushort area;              // ?

		public short minx;
		public short miny;
		public short minz;
		public short maxx;
		public short maxy;
		public short maxz;

		public ushort first_leaf_face;   // index of the first face (in the face leaf array)
		public ushort num_leaf_faces;    // number of consecutive edges (in the face leaf array)

		public ushort first_leaf_brush;  // ?
		public ushort num_leaf_brushes;  // ?

		public void Read(System.IO.BinaryReader source)
		{
			brush_or = source.ReadUInt32();
			cluster = source.ReadInt16();           // -1 for cluster indicates no visibility information
			area = source.ReadUInt16();              // ?

			minx = source.ReadInt16();
			miny = source.ReadInt16();
			minz = source.ReadInt16();
			maxx = source.ReadInt16();
			maxy = source.ReadInt16();
			maxz = source.ReadInt16();

			first_leaf_face = source.ReadUInt16();   // index of the first face (in the face leaf array)
			num_leaf_faces = source.ReadUInt16();    // number of consecutive edges (in the face leaf array)

			first_leaf_brush = source.ReadUInt16();  // ?
			num_leaf_brushes = source.ReadUInt16();  // ?
		}
	}
}
