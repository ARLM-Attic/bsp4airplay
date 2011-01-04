using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using BspFileFormat.Math;

namespace BspFileFormat.Q1HL1
{
	struct surface_t
	{
		Vector3 vectorS;            // S vector, horizontal in texture space)
		float distS;              // horizontal offset in texture space
		Vector3 vectorT;            // T vector, vertical in texture space
		float distT;              // vertical offset in texture space
		uint texture_id;         // Index of Mip Texture
		//           must be in [0,numtex[
		uint animated;           // 0 for ordinary textures, 1 for water 
	} ;
}
