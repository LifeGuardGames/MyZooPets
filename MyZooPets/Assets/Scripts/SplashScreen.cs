using UnityEngine;
using System.Collections;
using System;

public class SplashScreen : MonoBehaviour {

	public bool isDebug = false; //use to turn on splash screen 
	private float timer; //count down timer
	public float delayTime; //how long you want the splash screen to last
	public static bool IsFinished{get; set;} //use this method to check
											//if splash screen is done


	void Start(){
		guiTexture.pixelInset = new Rect(0,0,0,0);
		IsFinished = false;
		timer = delayTime;

		if(isDebug) guiTexture.color = Color.clear; //splash screen is transparent during debug
	}

	void Update()
	{
		timer -= Time.deltaTime; //decrease the delay time every frame
		if(timer > 0) return; //return if splash screen needs to hang longer

		FadeStartScene();
		if(guiTexture.color == Color.clear){
			IsFinished = true;
			Destroy(gameObject); //destroy splash screen when done
		}
	}

	//fades the splash screen
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
