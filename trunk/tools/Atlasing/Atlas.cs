using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Atlasing
{
	public class Atlas
	{
		Dictionary<Bitmap, AtlasItem> items = new Dictionary<Bitmap, AtlasItem>();
		bool atlasIsValid = false;
		public Atlas()
		{
		}

		public AtlasItem Add(Bitmap bitmap)
		{
			AtlasItem i;
			if (!items.TryGetValue(bitmap, out i))
			{
				i = new AtlasItem(this, bitmap);
				items[bitmap] = i;
				InvalidateAtlas();
			}
			return i;
		}

		private void InvalidateAtlas()
		{
			atlasIsValid = false;
		}
		private void BuildAtlas()
		{
			if (atlasIsValid)
				return;
			atlasIsValid = true;
		}
	}
}
