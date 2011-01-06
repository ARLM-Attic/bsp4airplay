using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;

namespace AirplaySDKFileFormats
{
	public class CTextWriter : IDisposable
	{
		System.IO.TextWriter writer;
		string filePath;

		bool isLineOpened;
		int depth = 0;

		public CTextWriter(string groupFilePath)
		{
			filePath = groupFilePath;
			writer = new StreamWriter(File.Create(filePath));
		}
		public void WriteLine(string text)
		{
			BeginWriteLine();
			writer.Write(text);
			EndWriteLine();
		}
		public void WriteKeyVal(string name, object val)
		{
			WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1}", name, val));
		}
		public void WriteString(string name, string val)
		{
			WriteKeyVal(name, string.Format("\"{0}\"", val.Replace("\"", "\\\"")));
		}
		public void OpenChild(string name)
		{
			WriteLine(name);
			WriteLine("{");
			++depth;
		}
		public void CloseChild()
		{
			--depth;
			WriteLine("}");
		}
		public void BeginWriteLine()
		{
			if (isLineOpened) return;
			isLineOpened = true;
			for (int i = 0; i < depth; ++i)
				writer.Write('\t');
		}
		public void EndWriteLine()
		{
			writer.Write('\n');
			isLineOpened = false;
		}

		public void Close()
		{
			while (depth > 0)
				CloseChild();
			if (writer != null)
				writer.Close();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (writer != null)
				writer.Dispose();
		}

		#endregion

		public void WriteVec3(string name, CIwVec3 val)
		{
			WriteKeyVal(name, string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}, {2}}}", val.x, val.y, val.z));
		}

		internal void WriteArray(string name, IList array)
		{
			BeginWriteLine();
			writer.Write(name);
			writer.Write(" ");
			writer.Write("{");
			if (array != null && array.Count > 0)
			{
				writer.Write(string.Format(CultureInfo.InvariantCulture, "{0}", array[0]));
				for (int i=1; i<array.Count; ++i)
					writer.Write(string.Format(CultureInfo.InvariantCulture, ", {0}", array[i]));
			}
			
			writer.Write("}");
			EndWriteLine();
		}
	}
}
