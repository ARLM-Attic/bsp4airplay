#pragma once
#include <IwResource.h>
#include <IwModel.h>

namespace Bsp4Airplay
{
	class Cb4aLeaf;
	class Cb4aLevel;

#ifdef IW_BUILD_RESOURCES
	void* Cb4aLeafFactory();

	class ParseLeaf: public CIwManaged
	{
		Cb4aLeaf* _this;
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

	class Cb4aLeaf
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLeaf;
#endif
	public:
		CIwArray<int32> visible_leaves;
		uint32 modelHash;
		int32 modelMesh;

		CIwModel* model;

		int32 cluster;
	public:

		//Constructor
		Cb4aLeaf();
		//Desctructor
		~Cb4aLeaf();

		void  Serialise ();

		void Render(Cb4aLevel*);
	};

}