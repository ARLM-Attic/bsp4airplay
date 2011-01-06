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
	//Cb4aLevel* model = static_cast<Cb4aLevel*>(group->GetResNamed("sg0503", "Cb4aLevel"));
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
			view.LookAt(CIwVec3(0,0,IW_GEOM_ONE),CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0));
			view.SetTrans(CIwVec3(512,512,1048));
			CIwMat modelMatrix;
			modelMatrix.SetIdentity();

			IwGxSetPerspMul(IwGxGetScreenWidth()/2);
			IwGxSetFarZNearZ(4096,16);

			IwGxSetViewMatrix(&view);
			IwGxSetModelMatrix(&modelMatrix);
			IwGxLightingOff();
			
			//model->Render();
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