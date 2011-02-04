using System;
using System.Collections.Generic;
using System.Text;

namespace ModelFileFormat.HL2
{
	/// <summary>
	/// File format is here: http://developer.valvesoftware.com/wiki/MDL
	/// </summary>
	class MdlReader : IModelReader
	{
		#region IModelReader Members

		public void ReadModel(System.IO.BinaryReader source, ModelDocument dest)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
