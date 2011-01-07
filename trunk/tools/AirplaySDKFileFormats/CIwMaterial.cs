namespace AirplaySDKFileFormats
{
	public class CIwMaterial : CIwResource
	{
		public string Texture0;
		public string Texture1;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteString("texture0", Texture0);
			writer.WriteString("texture1", Texture1);
		}
	}
}
