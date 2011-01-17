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

		public const uint SURFACE_HINT = (1 << 5);       // Make a primary BSP splitter
		public const uint SURFACE_DISCRETE = (1 << 6);       // Don't clip or merge this surface
		public const uint SURFACE_PORTALSKY = (1 << 7);       // This surface needs a portal sky render
		public const uint SURFACE_SLICK = (1 << 8);       // Entities should slide along this surface
		public const uint SURFACE_BOUNCE = (1 << 9);      // Entities should bounce off this surface
		public const uint SURFACE_LADDER = (1 << 10);      // Players can climb up this surface
		public const uint SURFACE_NODAMAGE = (1 << 11);      // Never give falling damage
		public const uint SURFACE_NOIMPACT = (1 << 12);      // Don't make impact effects
		public const uint SURFACE_NOSTEPS = (1 << 13);      // Don't play footstep sounds
		public const uint SURFACE_NOCLIP = (1 << 14);      // Don't generate a clip-surface at all
		public const uint SURFACE_NODRAW = (1 << 15);      // Don't generate a draw-surface at all
		public const uint SURFACE_NOTJUNC = (1 << 16);       // Don't use this surface for T-Junction fixing
	}
}
