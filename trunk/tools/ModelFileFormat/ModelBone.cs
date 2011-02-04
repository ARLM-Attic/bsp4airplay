using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

namespace ModelFileFormat
{
	public class ModelBone
	{
		public string Name;
		public int Parent;

		public Vector3 Position;
		public Quaternion Rotaton;

		public Vector3 WorldPosition;
		public Quaternion WorldRotaton;

		internal Vector3 Transform(Vector3 pos)
		{
			return Vector3.Transform(pos,WorldRotaton) + WorldPosition;
		}

		internal void EvalMatrix(List<ModelBone> modelBones)
		{
			WorldPosition = Position;
			WorldRotaton = Rotaton;
			var p = Parent;
			while (p >= 0)
			{
				var modelBone = modelBones[p];
				WorldPosition = Vector3.Transform(WorldPosition, modelBone.Rotaton) + modelBone.Position;
				WorldRotaton = modelBone.Rotaton * WorldRotaton;
				p = modelBone.Parent;
			}
		}
	}
}
