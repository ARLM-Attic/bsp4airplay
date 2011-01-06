using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aTreeElement : CIwParseable
	{
		public int Index;
		public float PlaneDistance;
		public CIwVec3 PlaneNormal;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
		}
	}
}
