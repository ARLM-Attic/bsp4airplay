#pragma once
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVertexBuffer;
	class Cb4aLevelVBSubcluster;
	class Cb4aLevel;
#ifdef IW_BUILD_RESOURCES
	void* Cb4aLevelVertexBufferFactory();

	class ParseLevelVertexBuffer: public CIwManaged
	{
		Cb4aLevelVertexBuffer* _this;
		public:
		// ---- Text resources ----
		// Parse from text file: start block.
		virtual void  ParseOpen (CIwTextParserITX *pParser);

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
	};
#endif

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
		CIwArray<Cb4aLevelVBSubcluster*> renderQueue;
	public:
		Cb4aLevelVertexBuffer();
		~Cb4aLevelVertexBuffer();
		void SetCapacity(uint32);
		void Serialise();
		void ScheduleCluster(Cb4aLevelVBSubcluster* );
		void Flush(Cb4aLevel* l);
		inline const CIwSVec3 & GetPosition(uint i) const {return positions[i];}
		inline const CIwSVec3 & GetNormal(uint i) const {return normals[i];}
		inline const CIwSVec2 & GetUV0(uint i) const {return uv0s[i];}
		inline const CIwSVec2 & GetUV1(uint i) const {return uv1s[i];}
		inline const CIwColour & GetColour(uint i) const {return colours[i];}
	protected:
		void FlushQueueBlock(Cb4aLevel* l,uint32 from, uint32 end);
		void FlushQueueDynamicBlock(Cb4aLevel* l,uint32 from, uint32 end);
		void PreRender();
		void PostRender();
	};
}