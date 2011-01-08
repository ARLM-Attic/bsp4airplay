#pragma once
#include <IwResource.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVBSubcluster;
	class Cb4aLevel;
#ifdef IW_BUILD_RESOURCES
	void* Cb4aLevelVBSubclusterFactory();

	class ParseLevelVBSubcluster: public CIwManaged
	{
		Cb4aLevelVBSubcluster* _this;
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

	class Cb4aLevelVBSubcluster
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLevelVBSubcluster;
#endif
	public:
		CIwArray<uint16> indices;
	public:

		//Constructor
		Cb4aLevelVBSubcluster();
		//Desctructor
		~Cb4aLevelVBSubcluster();

		void  Serialise ();

		void Render(Cb4aLevel*);
	};

}