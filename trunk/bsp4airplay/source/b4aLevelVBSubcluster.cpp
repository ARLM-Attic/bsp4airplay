#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include <IwModelBlock.h>
#include "b4aLevelVBSubcluster.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLevelVBSubcluster g_ParseLevelVBSubcluster;
#endif
}

//Constructor
Cb4aLevelVBSubcluster::Cb4aLevelVBSubcluster()
{
	sphere.SetRadius(0);
	sphere.t = CIwVec3::g_Zero;
	material= 0;
}

//Desctructor
Cb4aLevelVBSubcluster::~Cb4aLevelVBSubcluster()
{
}
bool Cb4aLevelVBSubcluster::IsVisible() const 
{ 
	uint16 res = IwGxClipSphere(sphere); 
	if (!res)
		return true;
	return false;
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLevelVBSubcluster::Serialise ()
{
	indices.SerialiseHeader();
	IwSerialiseUInt32(material);
	sphere.Serialise();
	for (uint32 i=0; i<indices.size(); ++i)
		IwSerialiseUInt16(indices[i]);

}
//void Cb4aLevelVBSubcluster::Render(Cb4aLevel* l)
//{
//	IwGxDrawPrims(IW_GX_TRI_LIST,&indices[0],indices.size());
//}
#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aLevelVBSubclusterFactory()
{
	return &g_ParseLevelVBSubcluster;
}

// Parse from text file: start block.
void  ParseLevelVBSubcluster::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	ParseLevelVBCluster* level = dynamic_cast<ParseLevelVBCluster*>(pParser->GetObject(-1));
	_this = level->AllocateSubcluster();
}
// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLevelVBSubcluster::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	
	if (!strcmp("material", pAttrName))
	{
		pParser->ReadUInt32(&_this->material);
		return true;
	}
	if (!strcmp("sphere_pos", pAttrName))
	{
		CIwVec3 pos;
		pParser->ReadInt32Array(&pos.x,3);
		_this->sphere.t = pos;
		return true;
	}
	if (!strcmp("sphere_r", pAttrName))
	{
		int32 pos;
		pParser->ReadInt32(&pos);
		_this->sphere.SetRadius(pos);
		return true;
	}
	if (!strcmp("num_indices", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->indices.set_capacity(n);
		return true;
	}
	if (!strcmp("t", pAttrName))
	{
		uint16 n;
		pParser->ReadUInt16(&n);
		_this->indices.push_back(n);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLevelVBSubcluster::ParseClose(CIwTextParserITX* pParser)
{
}
#endif