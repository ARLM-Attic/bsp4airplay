using System.Collections.Generic;
using BspFileFormat.BspMath;
namespace BspFileFormat
{
	public class BspCollisionConvexBrush : BspCollisionObject
	{
		public List<Plane> Planes = new List<Plane>();
	}
}
