using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aCollisionMeshSoupFaceEdge
	{
		public CIwVec3 Normal;
		public int Distance;
	}
	public class Cb4aCollisionMeshSoupFace
	{
		public CIwVec3 Normal;
		public int Distance;
		public List<Cb4aCollisionMeshSoupFaceEdge> edges = new List<Cb4aCollisionMeshSoupFaceEdge>();
	}
	public class Cb4aCollisionMeshSoup : CIwParseable, Ib4aCollider
	{
		public List<Cb4aCollisionMeshSoupFace> Faces = new List<Cb4aCollisionMeshSoupFace>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_faces",Faces.Count);
			foreach (var f in Faces)
			{
				writer.WriteLine("next_face");
				writer.WriteArray("face_p", new int[]{f.Normal.x,f.Normal.y,f.Normal.z, f.Distance});
				writer.WriteKeyVal("num_face_edges", f.edges.Count);
				foreach (var e in f.edges)
				{
					writer.WriteArray("edge_p", new int[] { e.Normal.x, e.Normal.y, e.Normal.z, e.Distance });
				}
			}
		}
	};
	public interface Ib4aCollider
	{
	};
}
