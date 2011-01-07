#include "Bsp4Airplay.h"
#include "b4aEntity.h"

void Bsp4Airplay::Bsp4AirpayInit()
{
	#ifdef IW_BUILD_RESOURCES
	//IwGetResManager()->AddHandler(new CResHandlerWAV);

	IwClassFactoryAdd("Cb4aLeaf", Cb4aLeafFactory, 0);
	IwClassFactoryAdd("Cb4aNode", Cb4aNodeFactory, 0);
	IwClassFactoryAdd("Cb4aEntity", Cb4aEntityFactory, 0);
	#endif

	IW_CLASS_REGISTER(Cb4aLevel);
}
void Bsp4Airplay::Bsp4AirpayTerminate()
{
}