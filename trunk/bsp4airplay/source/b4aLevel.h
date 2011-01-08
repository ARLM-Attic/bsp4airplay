#pragma once
#include <IwResource.h>
#include "b4aLeaf.h"
#include "b4aNode.h"
#include "b4aEntity.h"
#include "b4aLevelVB.h"
#include "b4aLevelVBCluster.h"

#define BSP4AIRPLAY_RESTYPE_LEVEL	"Cb4aLevel"

namespace Bsp4Airplay
{
	class Cb4aLevel : public CIwResource
	{
		Cb4aLevelVertexBuffer buffer;
		CIwArray<Cb4aLeaf> leaves;
		CIwArray<Cb4aNode> nodes;
		CIwArray<Cb4aEntity> entities;
		CIwArray<Cb4aLevelVBCluster> clusters;
	public:
		//Declare managed class
		IW_MANAGED_DECLARE(Cb4aLevel);

		//Constructor
		Cb4aLevel();
		//Desctructor
		virtual ~Cb4aLevel();

		virtual void  Serialise ();

		void Render();
		void Render(const CIwVec3 & viewer);
		void RenderCluster(int32 i);
		int FindEntityByClassName(const char* name) const;
		inline const Cb4aEntity* GetEntityAt(int32 i) const {return &entities[i];}

		// ---- Text resources ----
#ifdef IW_BUILD_RESOURCES
		Cb4aLeaf* AllocateLeaf();
		Cb4aNode* AllocateNode();
		Cb4aEntity* AllocateEntity();
		Cb4aLevelVBCluster* AllocateCluster();

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
#endif
	};
}