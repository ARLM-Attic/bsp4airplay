#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include <IwModelBlock.h>
#include "b4aLeaf.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLeaf g_parseLeaf;
#endif
}

//Constructor
Cb4aLeaf::Cb4aLeaf()
{
	modelHash = 0;
	model = 0;
	modelMesh = -1;
	cluster = -1;
}

//Desctructor
Cb4aLeaf::~Cb4aLeaf()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLeaf::Serialise ()
{
	IwSerialiseUInt32(modelHash);
	IwSerialiseInt32(modelMesh);
	IwSerialiseInt32(cluster);
	visible_leaves.SerialiseHeader();
	for (uint32 i=0; i<visible_leaves.size(); ++i)
		IwSerialiseInt32(visible_leaves[i]);

}

void Cb4aLeaf::Render(Cb4aLevel*l)
{
	if (!model)
	{
		if (!modelHash)
		{
			if (cluster<0)
				return;
			l->RenderCluster(cluster);
			return;
		}
		model = (CIwModel*)IwGetResManager()->GetResHashed(modelHash, "CIwModel");
		
	}
	
	if (modelMesh >= 0)
		static_cast<CIwModelBlock*>(model->m_Blocks[modelMesh])->Render(model, model->GetFlags());
	else
		model->Render();
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
	
	if (!strcmp("mesh", pAttrName))
	{
		pParser->ReadInt32(&_this->modelMesh);
		return true;
	}
	if (!strcmp("cluster", pAttrName))
	{
		pParser->ReadInt32(&_this->cluster);
		return true;
	}
	if (!strcmp("model", pAttrName))
	{
		pParser->ReadStringHash(&_this->modelHash);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLeaf::ParseClose(CIwTextParserITX* pParser)
{
}
#endif