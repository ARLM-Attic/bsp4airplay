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

void  Cb4aLevel::Serialise ()
{
	CIwResource::Serialise();

	leaves.SerialiseHeader();
	for (uint32 i=0; i<leaves.size(); ++i)
		leaves[i].Serialise();

	nodes.SerialiseHeader();
	for (uint32 i=0; i<nodes.size(); ++i)
		nodes[i].Serialise();

	entities.SerialiseHeader();
	for (uint32 i=0; i<entities.size(); ++i)
		entities[i].Serialise();
}
int Cb4aLevel::FindEntityByClassName(const char* name) const
{
	for (uint32 i=0; i<entities.size(); ++i)
		if (entities[i].GetClassName() == name)
			return (int)i;
	return -1;
}
void Cb4aLevel::Render(const CIwVec3 & viewer)
{
	CIwMat modelMatrix;
	modelMatrix.SetIdentity();
	IwGxSetModelMatrix(&modelMatrix);
	IwGxLightingOff();

	int node = 0;
	while (!nodes[node].WalkNode(viewer, &node));

	leaves[node].Render();
	for (uint32 i=0;i<leaves[node].visible_leaves.size(); ++i)
		leaves[leaves[node].visible_leaves[i]].Render();
}
void Cb4aLevel::Render()
{
	const CIwMat& m = IwGxGetViewMatrix();
	Render(m.t);
}
#ifdef IW_BUILD_RESOURCES
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
Cb4aEntity* Cb4aLevel::AllocateEntity()
{
	entities.push_back();
	return &entities.back();
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
	if (!strcmp("num_entities", pAttrName))
	{
		int num_nodes;
		pParser->ReadInt32(&num_nodes);
		entities.set_capacity(num_nodes);
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
#endif