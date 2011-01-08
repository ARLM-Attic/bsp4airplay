
using AirplaySDKFileFormats.Model;
using System.Collections.Generic;
namespace AirplaySDKFileFormats
{
	public class CIwModel : CIwResource
	{
		public List<CMesh> ModelBlocks = new List<CMesh>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			foreach (var mesh in ModelBlocks)
				mesh.WrtieToStream(writer);
		}
	}
}
