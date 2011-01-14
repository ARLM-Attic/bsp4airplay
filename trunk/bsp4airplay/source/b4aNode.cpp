#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLevel.h"
#include "b4aNode.h"
#include "Bsp4Airplay.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseNode g_parseNode;
#endif
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
	plane.v.Serialise();
	IwSerialiseInt32(plane.k);
	IwSerialiseBool(is_front_leaf);
	IwSerialiseInt32(front);
	IwSerialiseBool(is_back_leaf);
	IwSerialiseInt32(back);
}
bool Cb4aNode::WalkNode(const CIwSVec3 & viewer, int32* nextNode) const
{
	
	bool positive = plane.v*viewer >= plane.k;
	if (positive)
	{
		*nextNode = front;
		return is_front_leaf;
	}
	else
	{
		*nextNode = back;
		return is_back_leaf;
	}
}
bool Cb4aNode::TraceFrontLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	if (is_front_leaf)
		return l->GetLeaf(front).TraceLine(l,context);
	return l->GetNode(front).TraceLine(l,context);
}
bool Cb4aNode::TraceBackLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	if (is_back_leaf)
		return l->GetLeaf(back).TraceLine(l,context);
	return l->GetNode(back).TraceLine(l,context);
}
bool Cb4aNode::TraceLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	iwfixed fromDist = b4aPlaneDist(context.from,plane);
	iwfixed toDist = b4aPlaneDist(context.to,plane);
	if (fromDist >= 0 && toDist >= 0)
		return TraceFrontLine(l,context);
	if (fromDist < 0 && toDist < 0)
		return TraceBackLine(l,context);

	if (fromDist >= 0)
	{
		if (TraceFrontLine(l,context))
			return true;
		return TraceBackLine(l,context);
	}
	if (TraceBackLine(l,context))
		return true;
	return TraceFrontLine(l,context);
	//CIwSVec3 middlePoint = b4aLerp(context.from,context.to,fromDist/4096,toDist/4096);
	//if (fromDist >= 0)
	//{
	//	Cb4aTraceContext temp = context;
	//	temp.to = middlePoint;
	//	if (TraceFrontLine(l,temp))
	//	{
	//		context.to = temp.to;
	//		context.collisionNormal = temp.collisionNormal;
	//		return true;
	//	}
	//	temp.from = middlePoint;
	//	temp.to = context.to;
	//	if (TraceBackLine(l,temp))
	//	{
	//		context.to = temp.to;
	//		context.collisionNormal = temp.collisionNormal;
	//		return true;
	//	}
	//	return false;
	//}
	//Cb4aTraceContext temp = context;
	//temp.to = middlePoint;
	//if (TraceBackLine(l,temp))
	//{
	//	context.to = temp.to;
	//	context.collisionNormal = temp.collisionNormal;
	//	return true;
	//}
	//temp.from = middlePoint;
	//temp.to = context.to;
	//if (TraceFrontLine(l,temp))
	//{
	//	context.to = temp.to;
	//	context.collisionNormal = temp.collisionNormal;
	//	return true;
	//}
	//return false;
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
	if (!strcmp("plane", pAttrName))
	{
		iwfixed planeValues[4];
		pParser->ReadInt32Array(&planeValues[0],4);
		_this->plane = CIwPlane(CIwSVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
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