#pragma once
#include <IwManaged.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVertexBuffer;
	class Cb4aLevelVBSubcluster;
	class Cb4aLevel;

	class Ib4aProjection
	{
		public:

		//Constructor
		inline Ib4aProjection(){};
		//Desctructor
		inline virtual ~Ib4aProjection(){};

		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)=0;
		virtual void Flush()=0;
	};

	class Cb4aFlashlightProjection:public Ib4aProjection
	{
		CIwTexture* texure;
		CIwMat matrix;
		CIwVec3 whz;
		CIwBBox projectionBox;
		CIwArray<CIwSVec3> positions;
		CIwArray<CIwSVec2> uv0;
		CIwArray<uint16> indices;
	public:
		Cb4aFlashlightProjection();
		void Prepare(CIwTexture* tex, const CIwMat& mat, const CIwVec3 & _whz);
		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry);
		virtual void Flush();
	};
	class Cb4aSkyProjection:public Ib4aProjection
	{
	public:
		Cb4aSkyProjection();
		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry);
		virtual void Flush();
	};
}