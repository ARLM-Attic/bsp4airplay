using BspFileFormat.BspMath;
using System.Collections.Generic;
namespace BspFileFormat
{
	public class BspCollisionFaceSoupFaceEdge
	{
		public Vector3 Normal;
		public float Distance;
	}
	public class BspCollisionFaceSoupFace
	{
		public Vector3 Normal;
		public float Distance;
		public List<BspCollisionFaceSoupFaceEdge> Edges = new List<BspCollisionFaceSoupFaceEdge>();
	}
	public class BspCollisionFaceSoupEdge
	{
	}
	public class BspCollisionFaceSoup : BspCollisionObject
	{
		public List<BspCollisionFaceSoupFace> Faces = new List<BspCollisionFaceSoupFace>();
		public List<BspCollisionFaceSoupEdge> Edges = new List<BspCollisionFaceSoupEdge>();
		public List<Vector3> Vertices = new List<Vector3>();
	}
}
