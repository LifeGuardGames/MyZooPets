using UnityEngine;
using System.Collections;
using System;

public class SplashScreen : MonoBehaviour {

	private float timer; //count down timer
	public float delayTime; //how long you want the splash screen to last

	void Awake(){
		timer = delayTime;
	}

	void Update()
	{
		timer -= Time.deltaTime; //decrease the delay time every frame
		if(timer > 0) return; //return if splash screen needs to hang longer

		FadeStartScene();
	}

	//fades the splash screen
	void FadeStartScene()
	{
		Application.LoadLevel("MenuScene");
	}
}
