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
	defaultTextureHash = IwHashString("checkers");
}

//Desctructor
Cb4aLevel::~Cb4aLevel()
{
}

void  Cb4aLevel::Serialise ()
{
	CIwResource::Serialise();

	IwSerialiseUInt32(defaultTextureHash);

	materials.SerialiseHeader();
	for (uint32 i=0; i<materials.size(); ++i)
		materials[i].Serialise();

	buffers.SerialiseHeader();
	for (uint32 i=0; i<buffers.size(); ++i)
		buffers[i].Serialise();

	clusters.SerialiseHeader();
	for (uint32 i=0; i<clusters.size(); ++i)
		clusters[i].Serialise();

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
CIwTexture* Cb4aLevel::GetDefaultTextrure()
{
	return (CIwTexture*)IwGetResManager()->GetResHashed(defaultTextureHash, "CIwTexture", IW_RES_PERMIT_NULL_F | IW_RES_SEARCH_ALL_F);
}
int Cb4aLevel::FindEntityByClassName(const char* name, int startFrom) const
{
	for (uint32 i=startFrom; i<entities.size(); ++i)
		if (entities[i].GetClassName() == name)
			return (int)i;
	return -1;
}
void Cb4aLevel::RenderCluster(int32 i)
{
	if (i < 0) return;
	clusters[i].Render(this);
}
void Cb4aLevel::Render(const CIwVec3 & viewer)
{
	static CIwTexture* t = 0;
	if (!t)
		t = (CIwTexture*)IwGetResManager()->GetResNamed("checkers",IW_GX_RESTYPE_TEXTURE,IW_RES_PERMIT_NULL_F);
	CIwMat modelMatrix;
	modelMatrix.SetIdentity();

	IwGxSetModelMatrix(&modelMatrix);
	IwGxLightingOff();

	int node = 0;
	while (!nodes[node].WalkNode(viewer, &node));
	CIwMaterial* m = IW_GX_ALLOC_MATERIAL();
	if (t)
		m->SetTexture(t);
	IwGxSetMaterial(m);
	leaves[node].Render(this);
	for (uint32 i=0;i<leaves[node].visible_leaves.size(); ++i)
		leaves[leaves[node].visible_leaves[i]].Render(this);
	for (uint32 i=0;i<buffers.size(); ++i)
		buffers[i].Flush(this);
}
void Cb4aLevel::ScheduleRender(int32 i, Cb4aLevelVBSubcluster*c)
{
	buffers[i].ScheduleCluster(c);
}
void Cb4aLevel::Render()
{
	const CIwMat& m = IwGxGetViewMatrix();
	Render(m.t);
}
bool Cb4aLevel::TraceLine(Cb4aTraceContext& context) const
{
	if (nodes.empty())
		return false;
	return nodes[0].TraceLine(this, context);
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
Cb4aLevelVBCluster* Cb4aLevel::AllocateCluster()
{
	clusters.push_back();
	return &clusters.back();
}
Cb4aLevelVertexBuffer* Cb4aLevel::AllocateLevelVertexBuffer()
{
	buffers.push_back();
	return &buffers.back();
}
Cb4aLevelMaterial* Cb4aLevel::AllocateLevelMaterial()
{
	materials.push_back();
	return &materials.back();
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
	if (!strcmp("num_materials", pAttrName))
	{
		int num_materials;
		pParser->ReadInt32(&num_materials);
		materials.set_capacity(num_materials);
		return true;
	}
	if (!strcmp("num_clusters", pAttrName))
	{
		uint32 num_clusters;
		pParser->ReadUInt32(&num_clusters);
		clusters.set_capacity(num_clusters);
		return true;
	}
	if (!strcmp("num_vbs", pAttrName))
	{
		uint32 num_vbs;
		pParser->ReadUInt32(&num_vbs);
		buffers.set_capacity(num_vbs);
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