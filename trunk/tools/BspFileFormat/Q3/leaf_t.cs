using BspFileFormat.BspMath;

namespace BspFileFormat.Q3
{
	public class leaf_t
	{
		public int cluster;
		public int area;
		public int minx;
		public int miny;
		public int minz;
		public int maxx;
		public int maxy;
		public int maxz;

		public int leafface;
		public int n_leaffaces;
		public int leafbrush;
		public int n_leafbrushes;
		public void Read(System.IO.BinaryReader source)
		{
			cluster = source.ReadInt32();
			area = source.ReadInt32();
			minx = source.ReadInt32();
			miny = source.ReadInt32();
			minz = source.ReadInt32();
			maxx = source.ReadInt32();
			maxy = source.ReadInt32();
			maxz = source.ReadInt32();

			leafface = source.ReadInt32();
			n_leaffaces = source.ReadInt32();
			leafbrush = source.ReadInt32();
			n_leafbrushes = source.ReadInt32();
		}
	}
}
