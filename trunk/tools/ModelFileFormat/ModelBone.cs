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

		public Vector3 Position;
		public Quaternion Rotaton;

		public Vector3 WorldPosition;
		public Quaternion WorldRotaton;

		public void EvalMatrix(ModelBone modelBone)
		{
			WorldPosition = Vector3.Transform(Position,modelBone.WorldRotaton) + modelBone.WorldPosition;
			//WorldRotaton = Rotaton * modelBone.WorldRotaton;
			WorldRotaton = modelBone.WorldRotaton * Rotaton;
		}
		public void EvalMatrix()
		{
			WorldPosition = Position;
			WorldRotaton = Rotaton;
		}

		internal Vector3 Transform(Vector3 pos)
		{
			return Vector3.Transform(pos,WorldRotaton) + WorldPosition;
		}
	}
}
