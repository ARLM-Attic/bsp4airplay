#pragma once
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVertexBuffer
	{
		CIwGxStream sharedStream;
		CIwGxStream positionsStream;
		CIwGxStream normalsStream;
		CIwGxStream uv0Stream;
		CIwGxStream uv1Stream;
		CIwGxStream coloursStream;
	public:
		CIwArray<CIwSVec3> positions;
		CIwArray<CIwSVec3> normals;
		CIwArray<CIwSVec2> uv0s;
		CIwArray<CIwSVec2> uv1s;
		CIwArray<CIwColour> colours;
	public:
		Cb4aLevelVertexBuffer();
		~Cb4aLevelVertexBuffer();
		void SetCapacity(uint32);
		void Serialise();
		void PreRender();
		void PostRender();
	};
}