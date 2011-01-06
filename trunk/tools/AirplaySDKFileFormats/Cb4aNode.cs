using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aNode : Cb4aTreeElement
	{
		public bool IsFrontLeaf;
		public int Front;
		public bool IsBackLeaf;
		public int Back;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("plane_distance", (int)(PlaneDistance * AirplaySDKMath.IW_GEOM_ONE));
			writer.WriteVec3("plane_normal", PlaneNormal);
			writer.WriteKeyVal("is_front_leaf", IsFrontLeaf);
			writer.WriteKeyVal("front", Front);
			writer.WriteKeyVal("is_back_leaf", IsBackLeaf);
			writer.WriteKeyVal("back", Back);
		}
	}
}
