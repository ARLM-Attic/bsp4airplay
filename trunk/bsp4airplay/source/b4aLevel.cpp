#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLevel.h"

using namespace Bsp4Airplay;

IW_CLASS_FACTORY(Cb4aLevel);
IW_MANAGED_IMPLEMENT(Cb4aLevel)

//Constructor
Cb4aLevel::Cb4aLevel()
{
}

//Desctructor
Cb4aLevel::~Cb4aLevel()
{
}

Cb4aLeaf* Cb4aLevel::AllocateLeaf()
{
	leaves.push_back();
	return &leaves.back();
}
Cb4aNode* Cb4aLevel::AllocateNode()
{
	nodes.push_back();
	return &nodes.back();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool Cb4aLevel::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_leaves", pAttrName))
	{
		int num_leaves;
		pParser->ReadInt32(&num_leaves);
		leaves.set_capacity(num_leaves);
		return true;
	}
	if (!strcmp("num_nodes", pAttrName))
	{
		int num_nodes;
		pParser->ReadInt32(&num_nodes);
		nodes.set_capacity(num_nodes);
		return true;
	}
	
	return CIwResource::ParseAttribute(pParser, pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void Cb4aLevel::ParseClose(CIwTextParserITX* pParser)
{
	// Return value to resource Build() method
	pParser->SetReturnValue(this);
	CIwResManager* manager = IwGetResManager();
	CIwResGroup* group = manager->GetCurrentGroup();
	if (group)
		group->AddRes(BSP4AIRPLAY_RESTYPE_LEVEL, this);
}
