#pragma once
#include <IwResource.h>
#include <IwGx.h>
#include "Ib4aCollider.h"

namespace Bsp4Airplay
{
	class Cb4aNode
	{
	public:
		CIwPlane plane;
		bool is_front_leaf;
		int32 front;
		bool is_back_leaf;
		int32 back;

		//Constructor
		Cb4aNode();
		//Desctructor
		~Cb4aNode();

		void  Serialise ();

		bool WalkNode(const CIwVec3 & viewer, int32* nextNode) const;
		bool TraceLine(const Cb4aLevel*, Cb4aTraceContext& context) const;
		bool TraceSphere(const Cb4aLevel*, int32 r, Cb4aTraceContext& context) const;
	protected:
		bool TraceFrontLine(const Cb4aLevel*, Cb4aTraceContext& context) const;
		bool TraceBackLine(const Cb4aLevel*, Cb4aTraceContext& context) const;

		bool TraceFrontSphere(const Cb4aLevel*, int32 sphere, Cb4aTraceContext& context) const;
		bool TraceBackSphere(const Cb4aLevel*, int32 sphere, Cb4aTraceContext& context) const;
	};

#ifdef IW_BUILD_RESOURCES
	void* Cb4aNodeFactory();

	class ParseNode: public CIwManaged
	{
		Cb4aNode* _this;
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
}