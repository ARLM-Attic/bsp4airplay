#include <IwTextParserITX.h>
#include "b4aCollisionMeshSoup.h"
#include "Bsp4Airplay.h"

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
		faces[i].plane.v.Serialise();
		IwSerialiseInt32(faces[i].plane.k);

		
		faces[i].edges.SerialiseHeader();
		for (uint32 j=0; j<faces[i].edges.size(); ++j)
		{
			faces[i].edges[j].plane.v.Serialise();
			IwSerialiseInt32(faces[i].edges[j].plane.k);
		}
	}
}
bool Cb4aCollisionMeshSoupFace::TraceLine(Cb4aTraceContext& context) const
{
	iwfixed fromDist = b4aPlaneDist(context.from,plane);
	if (fromDist < -b4aCollisionEpsilon)
		return false;
	iwfixed toDist = b4aPlaneDist(context.to,plane);
	if (toDist > b4aCollisionEpsilon)
		return false;
	if (fromDist < toDist)
		return false;
	CIwSVec3 point = b4aLerp(context.from,context.to,fromDist,toDist);
	for (uint32 j=0; j<edges.size(); ++j)
	{
		const Cb4aCollisionMeshSoupFaceEdge& e = edges[j];
		if (b4aPlaneDist(point,e.plane)<-b4aCollisionEpsilon)
			return false;
	}
	context.to = point;
	context.collisionNormal = plane.v;
	return true;
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
		iwfixed planeValues[4];
		pParser->ReadInt32Array(&planeValues[0],4);
		faces.back().plane = CIwPlane(CIwSVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
		return true;
	}
	if (!strcmp("edge_p", pAttrName))
	{
		faces.back().edges.push_back();
		iwfixed planeValues[4];
		pParser->ReadInt32Array(&planeValues[0],4);
		faces.back().edges.back().plane = CIwPlane(CIwSVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
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