using BspFileFormat.BspMath;

namespace BspFileFormat.Q3
{

	public class node_t
	{
		public int planenum;
		public int front;
		public int back;
		public int minx;
		public int miny;
		public int minz;
		public int maxx;
		public int maxy;
		public int maxz;

		public void Read(System.IO.BinaryReader source)
		{
			planenum = source.ReadInt32();
			front = source.ReadInt32();
			back = source.ReadInt32();
			minx = source.ReadInt32();
			miny = source.ReadInt32();
			minz = source.ReadInt32();
			maxx = source.ReadInt32();
			maxy = source.ReadInt32();
			maxz = source.ReadInt32();
		}
	}
}
