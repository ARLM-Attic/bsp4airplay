#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLevel.h"
#include "b4aNode.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
	ParseNode g_parseNode;
}

//Constructor
Cb4aNode::Cb4aNode()
{
}

//Desctructor
Cb4aNode::~Cb4aNode()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aNode::Serialise ()
{
}

#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aNodeFactory()
{
	return &g_parseNode;
}

// Parse from text file: start block.
void  ParseNode::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateNode();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseNode::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("plane_distance", pAttrName))
	{
		pParser->ReadInt32(&_this->plane_distance);
		return true;
	}
	if (!strcmp("plane_normal", pAttrName))
	{
		pParser->ReadInt32Array(&_this->plane_normal.x,3);
		return true;
	}
	if (!strcmp("is_front_leaf", pAttrName))
	{
		pParser->ReadBool(&_this->is_front_leaf);
		return true;
	}
	if (!strcmp("front", pAttrName))
	{
		pParser->ReadInt32(&_this->front);
		return true;
	}
	if (!strcmp("is_back_leaf", pAttrName))
	{
		pParser->ReadBool(&_this->is_back_leaf);
		return true;
	}
	if (!strcmp("back", pAttrName))
	{
		pParser->ReadInt32(&_this->back);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseNode::ParseClose(CIwTextParserITX* pParser)
{
}

#endif