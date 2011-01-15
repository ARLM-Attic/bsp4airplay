#pragma once
#include <IwManaged.h>
#include <IwGx.h>
#include "Bsp4Airplay.h"
#include "Ib4aCollider.h"

namespace Bsp4Airplay
{
	struct Cb4aCollisionMeshSoupEdge
	{
		int32 length;
		CIwSVec3 point;
		CIwSVec3 offset;
	};
	struct Cb4aCollisionMeshSoupFaceEdge
	{
		Cb4aPlane plane;
		PlaneDistanceCalculator calc;
	};
	struct Cb4aCollisionMeshSoupFace
	{
		Cb4aPlane plane;
		PlaneDistanceCalculator calc;
		CIwArray<Cb4aCollisionMeshSoupFaceEdge> edges;

		bool TraceLine(Cb4aTraceContext& context) const;
		bool TraceSphere(int32 r, Cb4aTraceContext& context) const;
	};
	class Cb4aCollisionMeshSoup : public CIwManaged, public Ib4aCollider
	{
		CIwArray<Cb4aCollisionMeshSoupFace> faces;
		CIwArray<Cb4aCollisionMeshSoupEdge> edges;
		CIwArray<CIwSVec3> verices;
		public:
		//Declare managed class
		IW_MANAGED_DECLARE(Cb4aCollisionMeshSoup);

		//Constructor
		Cb4aCollisionMeshSoup();
		//Desctructor
		virtual ~Cb4aCollisionMeshSoup();

		virtual void  Serialise ();

		virtual bool TraceLine(Cb4aTraceContext& context) const ;
		virtual bool TraceSphere(int32 r, Cb4aTraceContext& context) const;
		bool TraceSphere(const Cb4aCollisionMeshSoupEdge& edge, int32 r, Cb4aTraceContext& context) const;
		bool TraceSphere(const CIwSVec3& v, int32 r, Cb4aTraceContext& context) const;
		// ---- Text resources ----
#ifdef IW_BUILD_RESOURCES
		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
#endif
	};
}