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

	buffer.Serialise();

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
int Cb4aLevel::FindEntityByClassName(const char* name) const
{
	for (uint32 i=0; i<entities.size(); ++i)
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
	buffer.PreRender();
	leaves[node].Render(this);
	for (uint32 i=0;i<leaves[node].visible_leaves.size(); ++i)
		leaves[leaves[node].visible_leaves[i]].Render(this);
	buffer.PostRender();
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
Cb4aLevelVBCluster* Cb4aLevel::AllocateCluster()
{
	clusters.push_back();
	return &clusters.back();
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
	if (!strcmp("num_vertices", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		buffer.SetCapacity(num_verts);
		return true;
	}
	if (!strcmp("num_clusters", pAttrName))
	{
		uint32 num_clusters;
		pParser->ReadUInt32(&num_clusters);
		clusters.set_capacity(num_clusters);
		return true;
	}
	if (!strcmp("v", pAttrName))
	{
		CIwSVec3 v;
		pParser->ReadInt16Array(&v.x,3);
		buffer.positions.push_back(v);
		return true;
	}
	if (!strcmp("vn", pAttrName))
	{
		CIwSVec3 vn;
		pParser->ReadInt16Array(&vn.x,3);
		buffer.normals.push_back(vn);
		return true;
	}
	if (!strcmp("uv0", pAttrName))
	{
		CIwSVec2 uv0;
		pParser->ReadInt16Array(&uv0.x,2);
		buffer.uv0s.push_back(uv0);
		return true;
	}
	if (!strcmp("uv1", pAttrName))
	{
		CIwSVec2 uv1;
		pParser->ReadInt16Array(&uv1.x,2);
		buffer.uv1s.push_back(uv1);
		return true;
	}
	if (!strcmp("col", pAttrName))
	{
		uint8 col[4];
		pParser->ReadUInt8Array(&col[0],4);
		CIwColour c;
		c.Set(col[0],col[1],col[2],col[3]);
		buffer.colours.push_back(c);
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