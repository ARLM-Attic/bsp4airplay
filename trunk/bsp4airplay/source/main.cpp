#include <s3e.h>
#include <s3ePointer.h>
#include <s3eKeyboard.h>
#include <IwGx.h>
#include <IwGraphics.h>
#include <Bsp4Airplay.h>

iwangle angleZ = 0;
iwangle angleBow = 0;
int32 oldMouseX = 0;
int32 oldMouseY = 0;
int32 isButtonDown = 0;
bool moveForward = false;
bool moveBackward = false;
bool moveLeft = false;
bool moveRight = false;
int32 pointerButton (s3ePointerEvent* systemData, void* userData)
{
	isButtonDown = (systemData->m_Pressed)?systemData->m_Button+1:0;
	oldMouseX = systemData->m_x;
	oldMouseY = systemData->m_y;
	return 0;
}

int32 pointerMotion (s3ePointerMotionEvent* systemData, void* userData)
{
	if (isButtonDown)
	{
		int32 dx = (systemData->m_x-oldMouseX)*4096/(int32)IwGxGetScreenWidth(); oldMouseX=systemData->m_x;
		int32 dy = (systemData->m_y-oldMouseY)*4096/(int32)IwGxGetScreenHeight(); oldMouseY=systemData->m_y;
		angleZ = (angleZ+dx) & 4095;
		angleBow = angleBow+dy;
		if (angleBow < -1000) angleBow = -1000;
		if (angleBow > 1000) angleBow = 1000;
	}
	return 0;
}
int32 pointerTouchMotion (s3ePointerTouchMotionEvent* systemData, void* userData)
{
	int32 dx = (systemData->m_x-oldMouseX)*4096/(int32)IwGxGetScreenWidth(); oldMouseX=systemData->m_x;
	int32 dy = (systemData->m_y-oldMouseY)*4096/(int32)IwGxGetScreenHeight(); oldMouseY=systemData->m_y;
	angleZ = (angleZ+dx) & 4095;
	angleBow = angleBow+dy;
	if (angleBow < -1000) angleBow = -1000;
	if (angleBow > 1000) angleBow = 1000;

	return 0;
}
int32 keyboardEvent (s3eKeyboardEvent* systemData, void* userData)
{
	switch (systemData->m_Key)
	{
	case s3eKeyW:
	case s3eKeyUp:
		moveForward = 0 != systemData->m_Pressed;
		break;
	case s3eKeyS:
	case s3eKeyDown:
		moveBackward = 0 != systemData->m_Pressed;
		break;
	case s3eKeyA:
	case s3eKeyLeft:
		moveLeft = 0 != systemData->m_Pressed;
		break;
	case s3eKeyD:
	case s3eKeyRight:
		moveRight = 0 != systemData->m_Pressed;
		break;
	}
	return 0;
}

//-----------------------------------------------------------------------------
// Main global function
//-----------------------------------------------------------------------------
int main()
{
	IwGxInit();
	IwResManagerInit();
	IwGraphicsInit();
	Bsp4Airplay::Bsp4AirpayInit();
	s3eKeyboardRegister(S3E_KEYBOARD_KEY_EVENT, (s3eCallback)keyboardEvent, 0);

	s3ePointerRegister(S3E_POINTER_BUTTON_EVENT, (s3eCallback)pointerButton, 0);
	s3ePointerRegister(S3E_POINTER_MOTION_EVENT, (s3eCallback)pointerMotion, 0);
	s3ePointerRegister(S3E_POINTER_TOUCH_MOTION_EVENT, (s3eCallback)pointerTouchMotion, 0);

	IwGxSetColClear(0x7f, 0x7f, 0x7f, 0x7f);
	IwGxPrintSetColour(128, 128, 128);

	CIwResGroup* group = IwGetResManager()->LoadGroup("maps/sg0503.group");
	Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("sg0503", "Cb4aLevel"));
	
	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/madcrabs.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("madcrabs", "Cb4aLevel"));

	int spawnEntIndex = level->FindEntityByClassName("info_player_start");
	if (spawnEntIndex < 0)
		spawnEntIndex = level->FindEntityByClassName("info_player_deathmatch");
	const Bsp4Airplay::Cb4aEntity* spawnEnt = (spawnEntIndex>=0)?level->GetEntityAt(spawnEntIndex):0;
	CIwVec3 cameraOrigin = spawnEnt ? (spawnEnt->GetOrigin()) : CIwVec3::g_Zero;


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

			IwGxClear(IW_GX_DEPTH_BUFFER_F);
			//IwGxClear(IW_GX_COLOUR_BUFFER_F | IW_GX_DEPTH_BUFFER_F);

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
			view.LookAt(CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0),CIwVec3(0,IW_GEOM_ONE,0));
			view.PreRotateY(angleZ);
			view.PreRotateX(angleBow);
			view.SetTrans(CIwVec3::g_Zero);

			CIwVec3 forward = view.RowZ(); forward.x /= (1<<10); forward.y /= (1<<10); forward.z /= (1<<10);
			CIwVec3 right = view.RowX(); right.x /= (1<<10); right.y /= (1<<10); right.z /= (1<<10);
			if (moveForward)
				cameraOrigin += forward;
			if (moveBackward)
				cameraOrigin -= forward;
			if (moveRight)
				cameraOrigin += right;
			if (moveLeft)
				cameraOrigin -= right;
			view.SetTrans(cameraOrigin);

			IwGxSetViewMatrix(&view);



			IwGxSetPerspMul(IwGxGetScreenWidth()/2);
			IwGxSetFarZNearZ(4096,16);

			level->Render(cameraOrigin);
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