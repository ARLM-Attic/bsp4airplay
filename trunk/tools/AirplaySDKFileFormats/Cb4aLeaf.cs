using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aLeaf : Cb4aTreeElement
	{
		public List<int> VisibleLeaves = new List<int>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_visible_leaves", VisibleLeaves.Count);
			foreach (var l in VisibleLeaves)
			{
				writer.WriteKeyVal("visible_leaf", l);
			}
		}
	}
}
