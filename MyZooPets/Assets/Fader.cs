using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {

	public bool isDebug = false;
	RoomGUI roomGui;
	private bool loaded = false;
	public static bool IsSplashScreenFinished{get; set;}

	void Awake()
	{
		guiTexture.pixelInset = new Rect(0,0,0,0);
		roomGui	= GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUI>();
		IsSplashScreenFinished = false;
	}


	void Update()
	{
		if (!loaded){
			if(isDebug){
				guiTexture.color = Color.clear;
				loaded = true;
				IsSplashScreenFinished = true;
			}

			FadeStartScene();
			if(guiTexture.color == Color.clear)
			{
				// roomGui.IntroFinished();
				loaded = true;
				IsSplashScreenFinished = true;
			}
		}
	}

	void FadeStartScene()
	{
		guiTexture.color = Color.Lerp(guiTexture.color,Color.clear,0.9f * Time.deltaTime);
		if(guiTexture.color.a <= 0.05f)
		{
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
		}
	}
}
