using System.Drawing;
using System.IO;
namespace AirplaySDKFileFormats
{
	public class CIwTexture : CIwResource
	{
		public string FilePath;
		public Bitmap Bitmap;

		public override void WrtieToStream(CTextWriter writer)
		{
			writer.WriteLine(string.Format("\"{0}\"", FilePath.Replace("\"", "\\\"")));
			if (Bitmap != null)
			{
				Bitmap.Save(Path.Combine(Path.GetDirectoryName(writer.FileName), FilePath));
			}
		}
	}
}
