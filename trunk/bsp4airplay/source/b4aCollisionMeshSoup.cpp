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
		if (IwSerialiseIsReading())
			faces[i].calc = GetDistanceCalculator(faces[i].plane);

		IwSerialiseInt32(faces[i].plane.k);

		faces[i].edges.SerialiseHeader();
		for (uint32 j=0; j<faces[i].edges.size(); ++j)
		{
			faces[i].edges[j].plane.v.Serialise();
			IwSerialiseInt32(faces[i].edges[j].plane.k);

			if (IwSerialiseIsReading())
				faces[i].edges[j].calc = GetDistanceCalculator(faces[i].edges[j].plane);
		}
	}
	edges.SerialiseHeader();
	for (uint32 i=0; i<edges.size(); ++i)
	{
		IwSerialiseInt32(edges[i].length);
		edges[i].point.Serialise();
		edges[i].offset.Serialise();
	}
	verices.SerialiseHeader();
	for (uint32 i=0; i<verices.size(); ++i)
		verices[i].Serialise();

}
bool Cb4aCollisionMeshSoupFace::TraceSphere(int32 sphere, Cb4aTraceContext& context) const
{
	CIwVec3 shift = CIwVec3(plane.v)*(-sphere);
	
	iwfixed fromDist = calc(context.from+shift,plane);
	if (fromDist < -b4aCollisionEpsilon)
		return false;
	iwfixed toDist = calc(context.to+shift,plane);
	if (toDist > b4aCollisionEpsilon)
		return false;
	if (fromDist <= toDist)
		return false;
	CIwVec3 point;
	if (fromDist <= 0)
		point = context.from;
	else if (toDist >= 0)
		point = context.to;
	else
		point = b4aLerp(context.from,context.to,fromDist,toDist);
	for (uint32 j=0; j<edges.size(); ++j)
	{
		const Cb4aCollisionMeshSoupFaceEdge& e = edges[j];
		if (e.calc(point,e.plane)<-b4aCollisionEpsilon-(sphere)) //-sphere makes collision be like bounding box instead of sphere. Cheap and simple trick
			return false;
	}
	context.to = point;
	context.collisionNormal = plane.v;
	context.collisionPlaneD = plane.k;
	return true;
}

bool Cb4aCollisionMeshSoupFace::TraceLine(Cb4aTraceContext& context) const
{
	iwfixed fromDist = calc(context.from,plane);
	if (fromDist < -b4aCollisionEpsilon)
		return false;
	iwfixed toDist = calc(context.to,plane);
	if (toDist > b4aCollisionEpsilon)
		return false;
	if (fromDist <= toDist)
		return false;
	CIwVec3 point;
	if (fromDist <= 0)
		point = context.from;
	else if (toDist >= 0)
		point = context.to;
	else
		point = b4aLerp(context.from,context.to,fromDist,toDist);
	for (uint32 j=0; j<edges.size(); ++j)
	{
		const Cb4aCollisionMeshSoupFaceEdge& e = edges[j];
		if (e.calc(point,e.plane)<-b4aCollisionEpsilon)
			return false;
	}
	context.to = point;
	context.collisionNormal = plane.v;
	context.collisionPlaneD = plane.k;
	return true;
}
bool Cb4aCollisionMeshSoup::TraceSphere(const Cb4aCollisionMeshSoupEdge& edge, int32 r, Cb4aTraceContext& context) const
{
	//int32 fromDist = context.from
	return false;
}
bool Cb4aCollisionMeshSoup::TraceSphere(const CIwSVec3& v, int32 r, Cb4aTraceContext& context) const
{
	return false;
}
bool Cb4aCollisionMeshSoup::TraceSphere(int32 sphere, Cb4aTraceContext& context) const
{
	bool res = false;
	for (uint32 i=0; i<faces.size(); ++i)
	{
		res |= faces[i].TraceSphere(sphere,context);
	}
	//This could be implemented for precise detection. For now I use a simple trick -(sphere<<IW_GEOM_POINT)
	//for (uint32 i=0; i<edges.size(); ++i)
	//{
	//	res |= TraceSphere(edges[i],sphere,context);
	//}
	//for (uint32 i=0; i<verices.size(); ++i)
	//{
	//	res |= TraceSphere(verices[i],sphere,context);
	//}
	return res;
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
	if (!strcmp("num_vertices", pAttrName))
	{
		int num_vertices;
		pParser->ReadInt32(&num_vertices);
		verices.set_capacity(num_vertices);
		return true;
	}
	if (!strcmp("v", pAttrName))
	{
		verices.push_back();
		pParser->ReadInt16Array(&verices.back().x,3);
		return true;
	}
	if (!strcmp("num_edges", pAttrName))
	{
		int num_edges;
		pParser->ReadInt32(&num_edges);
		edges.set_capacity(num_edges);
		return true;
	}
	if (!strcmp("e", pAttrName))
	{
		edges.push_back();
		pParser->ReadInt32(&edges.back().length);
		pParser->ReadInt16Array(&edges.back().point.x,3);
		pParser->ReadInt16Array(&edges.back().offset.x,3);
		return true;
	}
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
		faces.back().plane = Cb4aPlane(CIwVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
		return true;
	}
	if (!strcmp("edge_p", pAttrName))
	{
		faces.back().edges.push_back();
		iwfixed planeValues[4];
		pParser->ReadInt32Array(&planeValues[0],4);
		faces.back().edges.back().plane = Cb4aPlane(CIwVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
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