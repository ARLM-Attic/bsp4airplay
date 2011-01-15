#pragma once
#include <IwManaged.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	struct Cb4aTraceContext
	{
		CIwVec3 from;
		CIwVec3 to;
		CIwSVec3 collisionNormal;
		iwfixed collisionPlaneD;
	};

	class Ib4aCollider
	{
		public:

		//Constructor
		inline Ib4aCollider(){};
		//Desctructor
		inline virtual ~Ib4aCollider(){};

		virtual bool TraceLine(Cb4aTraceContext& context) const =0;
		virtual bool TraceSphere(int32 sphere, Cb4aTraceContext& context) const=0;
	};
	class Ib4aColliderContainer
	{
	public:
		inline Ib4aColliderContainer() {}
		inline virtual ~Ib4aColliderContainer() {}

		virtual void AddCollider(Ib4aCollider*)=0;
	};
}