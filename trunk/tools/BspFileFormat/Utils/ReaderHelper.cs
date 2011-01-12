using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.BspMath;
using System.Globalization;
using System.IO;

namespace BspFileFormat.Utils
{
	public static class ReaderHelper
	{
		public static void BuildEntities(string entities, BspDocument dest)
		{
			var lines = entities.Split(new char[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
			BspEntity entity = null;
			foreach (var rawline in lines)
			{
				var line = rawline.Trim();
				if (line == "{")
				{
					entity = new BspEntity();
					dest.Entities.Add(entity);
					continue;
				}
				if (line == "}")
				{
					entity = null;
					continue;
				}
				int keyStartsAt = line.IndexOf('\"')+1;
				if (keyStartsAt <= 0)
					continue;
				int keyEndsAt = line.IndexOf('\"', keyStartsAt);
				int valueStartsAt = line.IndexOf('\"', keyEndsAt+1)+1;
				var key = line.Substring(keyStartsAt, keyEndsAt - keyStartsAt);
				var val = line.Substring(valueStartsAt, line.Length - 1 - valueStartsAt);
				if (key == "classname")
				{
					entity.ClassName = val;
				}
				else if (key == "origin")
				{
					var vals = val.Split(new char[]{' '});
					entity.Origin = new Vector3(
						float.Parse(vals[0], CultureInfo.InvariantCulture),
						float.Parse(vals[1], CultureInfo.InvariantCulture),
						float.Parse(vals[2], CultureInfo.InvariantCulture)
						);
				}
				else
				{
					entity.Values.Add(new KeyValuePair<string, string>(key,val));
				}
			}
		}

		public static List<T> ReadStructs<T>(System.IO.BinaryReader source, uint size, long offset, uint itemSize) where T:new()
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			if (size % itemSize != 0)
				throw new ArgumentException("Wrong size "+itemSize+" for " + typeof(T).Name);
			int numItems = (int)(size / itemSize);
			var planes = new List<T>(numItems);
			var Read = typeof(T).GetMethod("Read");
			if (Read == null)
				throw new ArgumentException("No Read(stream) method in "+typeof(T).Name);
			for (int i = 0; i < numItems; ++i)
			{
				var v = new T();
				Read.Invoke(v, new object[] { source });
				//v.Read(source);
				planes.Add(v);
			}
			if (source.BaseStream.Position != size + offset)
				throw new ArgumentException("Wrong item size for " + typeof(T).Name);
			return planes;
		}

		internal static ushort[] ReadUInt16Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 2);
			var listOfFaces = new ushort[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadUInt16();
			}
			return listOfFaces;
		}
		internal static uint[] ReadUInt32Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 4);
			var listOfFaces = new uint[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadUInt32();
			}
			return listOfFaces;
		}
		internal static int[] ReadInt32Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 4);
			var listOfFaces = new int[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadInt32();
			}
			return listOfFaces;
		}

		internal static string ReadStringSZ(BinaryReader source)
		{
			var buf = new List<byte>(16);
			for (; ; )
			{
				var b = source.ReadByte();
				if (b == 0) break;
				buf.Add(b);
			}
			return Encoding.ASCII.GetString(buf.ToArray());
		}
	}
}
