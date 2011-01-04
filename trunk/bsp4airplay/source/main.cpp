#include <s3e.h>
#include <IwGx.h>


//-----------------------------------------------------------------------------
// Main global function
//-----------------------------------------------------------------------------
int main()
{
	IwGxInit();
	IwResManagerInit();

	IwGxSetColClear(0xff, 0xff, 0xff, 0xff);
	IwGxPrintSetColour(128, 128, 128);

	//CIwResGroup* music = IwGetResManager()->LoadGroup("music.group");
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
			IwGxFlush();
			IwGxSwapBuffers();
		}
	}
	IwResManagerTerminate();
	IwGxTerminate();
	return 0;
}