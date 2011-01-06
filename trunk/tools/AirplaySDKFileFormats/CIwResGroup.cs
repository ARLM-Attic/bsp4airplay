using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats
{
	public class CIwResGroup: CIwManaged
	{
		protected CIwManagedList list = new CIwManagedList();

		public void AddChild(CIwResGroup pGroup)
		{
			throw new NotImplementedException();
		}
		public void AddRes(CIwResource pData)
		{
			list.Add(pData);
		}
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			foreach (var l in list)
			{
				l.WrtieToStream(writer);
			}
		}
	}
}
