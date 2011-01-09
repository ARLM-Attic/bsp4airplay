using System.Text;

namespace BspFileFormat.Q3
{
	public class texture_t
	{
		public string name;
		public uint flags;
		public uint contents;

		public void Read(System.IO.BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[]{' ','\0'});
			flags = source.ReadUInt32();
			contents = source.ReadUInt32();
		}
	}
}
