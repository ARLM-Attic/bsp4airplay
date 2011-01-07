
using AirplaySDKFileFormats.Model;
namespace AirplaySDKFileFormats
{
	public class CIwModel : CIwResource
	{
		public CMesh Mesh = new CMesh();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			Mesh.WrtieToStream(writer);
		}
	}
}
