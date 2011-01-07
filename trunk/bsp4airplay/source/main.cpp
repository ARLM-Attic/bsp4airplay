#include <s3e.h>
#include <IwGx.h>
#include <IwGraphics.h>
#include <Bsp4Airplay.h>


//-----------------------------------------------------------------------------
// Main global function
//-----------------------------------------------------------------------------
int main()
{
	IwGxInit();
	IwResManagerInit();
	IwGraphicsInit();
	Bsp4Airplay::Bsp4AirpayInit();

	IwGxSetColClear(0x7f, 0x7f, 0x7f, 0x7f);
	IwGxPrintSetColour(128, 128, 128);

	CIwResGroup* group = IwGetResManager()->LoadGroup("maps/sg0503.group");
	Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("sg0503", "Cb4aLevel"));
	int spawnEntIndex = level->FindEntityByClassName("info_player_start");
	const Bsp4Airplay::Cb4aEntity* spawnEnt = level->GetEntityAt(spawnEntIndex);
	{
	
		while (1)
		{
			s3eDeviceYield(0);
			s3eKeyboardUpdate();
			s3ePointerUpdate();

			bool result = true;
			if	(
				(result == false) ||
				(s3eKeyboardGetState(s3eKeyEsc) & S3E_KEY_STATE_DOWN) ||
				(s3eKeyboardGetState(s3eKeyAbsBSK) & S3E_KEY_STATE_DOWN) ||
				(s3eDeviceCheckQuitRequest())
				)
				break;

			IwGxClear(IW_GX_COLOUR_BUFFER_F | IW_GX_DEPTH_BUFFER_F);

			CIwMat view;
			//if (spawnEnt >= 0)
			/*{
				const Bsp4Airplay::Cb4aEntity* e = level->GetEntityAt(spawnEnt);
				view.LookAt(CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0),CIwVec3(0,0,IW_GEOM_ONE));

				view.SetTrans(e->GetOrigin());
			}
			else
			{
				view.LookAt(CIwVec3(0,0,IW_GEOM_ONE),CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0));
				view.SetTrans(CIwVec3(512,512,1048));
			}*/
			view.LookAt(CIwVec3(0,0,IW_GEOM_ONE),CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0));
			view.SetTrans(CIwVec3(spawnEnt->GetOrigin().x,spawnEnt->GetOrigin().y,1048));

			IwGxSetViewMatrix(&view);


			IwGxSetPerspMul(IwGxGetScreenWidth()/2);
			IwGxSetFarZNearZ(4096,16);

			
			level->Render(spawnEnt->GetOrigin());
			IwGxFlush();
			IwGxSwapBuffers();
		}
	}
	Bsp4Airplay::Bsp4AirpayTerminate();
	IwGraphicsTerminate();
	IwResManagerTerminate();
	IwGxTerminate();
	return 0;
}