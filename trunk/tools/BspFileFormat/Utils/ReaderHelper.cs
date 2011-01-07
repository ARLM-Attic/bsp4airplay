using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.BspMath;
using System.Globalization;

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
	}
}
