#include <s3e.h>
#include <IwGx.h>
#include <IwGraphics.h>


//-----------------------------------------------------------------------------
// Main global function
//-----------------------------------------------------------------------------
int main()
{
	IwGxInit();
	IwResManagerInit();
	IwGraphicsInit();

	IwGxSetColClear(0x7f, 0x7f, 0x7f, 0x7f);
	IwGxPrintSetColour(128, 128, 128);

	CIwResGroup* group = IwGetResManager()->LoadGroup("models/sample.group");
	CIwModel* model = static_cast<CIwModel*>(group->GetResNamed("q1_0", "CIwModel"));

	//CTune* tune = static_cast<CTune*>(music->GetResNamed("drumkit", TUNE4AIRPLAY_RESTYPE_TUNE));
	//ISampler * sample = tune->GetSample(0);

	//int32 freq = (int32)sample->GetC5Speed();
	//s3eSoundSetInt(S3E_SOUND_DEFAULT_FREQ, freq);

	{
		//CTuneMixer mixer;
		//mixer.Play(tune);

		//int channel = s3eSoundGetFreeChannel();
		//s3eSoundChannelPlay(channel,sample->GetData(0),sample->GetSamples(),0,0);

		
		

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
			view.SetTrans(CIwVec3(0,0,2048));
			CIwMat modelMatrix;
			modelMatrix.SetIdentity();

			IwGxSetPerspMul(IwGxGetScreenWidth()/2);
			IwGxSetFarZNearZ(4096,16);

			IwGxSetViewMatrix(&view);
			IwGxSetModelMatrix(&modelMatrix);
			IwGxLightingOff();
			
			model->Render();
			IwGxFlush();
			IwGxSwapBuffers();
		}
	}
	IwGraphicsTerminate();
	IwResManagerTerminate();
	IwGxTerminate();
	return 0;
}