#pragma once
#include <IwResource.h>
#include "b4aLeaf.h"
#include "b4aNode.h"
#include "b4aEntity.h"
#include "b4aLevelVB.h"
#include "b4aLevelVBCluster.h"
#include "b4aLevelMaterial.h"

#define BSP4AIRPLAY_RESTYPE_LEVEL	"Cb4aLevel"

namespace Bsp4Airplay
{


	class Cb4aLevel : public CIwResource
	{
		CIwArray<Cb4aLevelVertexBuffer> buffers;
		CIwArray<Cb4aLeaf> leaves;
		CIwArray<Cb4aNode> nodes;
		CIwArray<Cb4aEntity> entities;
		CIwArray<Cb4aLevelMaterial> materials;
		CIwArray<Cb4aLevelVBCluster> clusters;
		uint32 defaultTextureHash;
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
		inline void BindMaterial(uint32 i) {materials[i].Bind(this);};

		bool TraceLine(Cb4aTraceContext& context) const;
		bool TraceSphere(int32 r, Cb4aTraceContext& context) const;

		inline const Cb4aNode& GetNode(uint i) const { return nodes[i];}
		inline const Cb4aLeaf& GetLeaf(uint i) const { return leaves[i];}
		
		int FindEntityByClassName(const char* name,int startFrom=0) const;
		inline uint32 GetNumEntities() const {return entities.size();}
		inline const Cb4aEntity* GetEntityAt(uint32 i) const {return &entities[i];}
		CIwTexture* GetDefaultTextrure();
		void ScheduleRender(int32 i, Cb4aLevelVBSubcluster*);
		// ---- Text resources ----
#ifdef IW_BUILD_RESOURCES
		Cb4aLeaf* AllocateLeaf();
		Cb4aNode* AllocateNode();
		Cb4aEntity* AllocateEntity();
		Cb4aLevelVBCluster* AllocateCluster();
		Cb4aLevelVertexBuffer* AllocateLevelVertexBuffer();
		Cb4aLevelMaterial* AllocateLevelMaterial();
		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
#endif
	};
}