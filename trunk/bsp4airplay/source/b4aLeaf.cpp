#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLeaf.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
	ParseLeaf g_parseLeaf;
}

//Constructor
Cb4aLeaf::Cb4aLeaf()
{
}

//Desctructor
Cb4aLeaf::~Cb4aLeaf()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLeaf::Serialise ()
{
}
#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aLeafFactory()
{
	return &g_parseLeaf;
}

// Parse from text file: start block.
void  ParseLeaf::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateLeaf();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLeaf::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_visible_leaves", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->visible_leaves.set_capacity(n);
		return true;
	}
	if (!strcmp("visible_leaf", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->visible_leaves.push_back(n);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLeaf::ParseClose(CIwTextParserITX* pParser)
{
}
#endif