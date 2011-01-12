#include <IwTextParserITX.h>
#include "b4aCollisionMeshSoup.h"

using namespace Bsp4Airplay;

IW_CLASS_FACTORY(Cb4aCollisionMeshSoup);
IW_MANAGED_IMPLEMENT(Cb4aCollisionMeshSoup)

//Constructor
Cb4aCollisionMeshSoup::Cb4aCollisionMeshSoup()
{
}

//Desctructor
Cb4aCollisionMeshSoup::~Cb4aCollisionMeshSoup()
{
}
void  Cb4aCollisionMeshSoup::Serialise ()
{
	faces.SerialiseHeader();
	for (uint32 i=0; i<faces.size(); ++i)
	{
		IwSerialiseInt32(faces[i].plane[0],4);
		faces[i].edges.SerialiseHeader();
		for (uint32 j=0; j<faces[i].edges.size(); ++j)
			IwSerialiseInt32(faces[i].edges[j].plane[0],4);
	}
}
bool Cb4aCollisionMeshSoupFace::TraceLine(Cb4aTraceContext& context) const
{
	return false;
}
bool Cb4aCollisionMeshSoup::TraceLine(Cb4aTraceContext& context) const 
{
	bool res = false;
	for (uint32 i=0; i<faces.size(); ++i)
	{
		res |= faces[i].TraceLine(context);
	}
	return res;
}
#ifdef IW_BUILD_RESOURCES

// function invoked by the text parser when parsing attributes for objects of this type
bool Cb4aCollisionMeshSoup::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_faces", pAttrName))
	{
		int num_faces;
		pParser->ReadInt32(&num_faces);
		faces.set_capacity(num_faces);
		return true;
	}
	if (!strcmp("num_face_edges", pAttrName))
	{
		int num_face_edges;
		pParser->ReadInt32(&num_face_edges);
		faces.back().edges.set_capacity(num_face_edges);
		return true;
	}
	if (!strcmp("num_face_edges", pAttrName))
	{
		int num_face_edges;
		pParser->ReadInt32(&num_face_edges);
		faces.back().edges.set_capacity(num_face_edges);
		return true;
	}
	if (!strcmp("next_face", pAttrName))
	{
		faces.push_back();
		return true;
	}
	if (!strcmp("face_p", pAttrName))
	{
		pParser->ReadInt32Array(&faces.back().plane[0],4);
		return true;
	}
	if (!strcmp("edge_p", pAttrName))
	{
		faces.back().edges.push_back();
		pParser->ReadInt32Array(&faces.back().edges.back().plane[0],4);
		return true;
	}

	
	return CIwManaged::ParseAttribute(pParser, pAttrName);
}


// function invoked by the text parser when the object definition end is encountered
void Cb4aCollisionMeshSoup::ParseClose(CIwTextParserITX* pParser)
{
	dynamic_cast<Ib4aColliderContainer*>(pParser->GetObject(-1))->AddCollider(this);
}
#endif