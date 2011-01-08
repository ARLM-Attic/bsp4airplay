#pragma once
#include <IwResource.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aNode
	{
	public:
		iwfixed plane_distance;
		CIwVec3	plane_normal;
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