using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Window controller.
/// This script is attached to the window object to control the display contents in the window.
/// </summary>
public class WindowController : MonoBehaviour {
	public Renderer[] windowImageRenderers; // The sun or the moon
	public Texture dayTexture;
	public Texture nightTexture;

	void Start(){
		CheckTime();
	}

	void OnApplicationPause(bool pauseStatus){
		if(!pauseStatus){
			CheckTime();
		}
	}

	private void CheckTime(){
		DateTime now = LgDateTime.GetTimeNow();
		if(now.Hour	>= 6 && now.Hour < 18){
			SetImageByTime(true);
		}
		else{
			SetImageByTime(false);
		}
	}

	private void SetImageByTime(bool isDaytime){
		if(isDaytime){
			// Set the day sprite
			foreach(Renderer rendererObject in windowImageRenderers){
				rendererObject.material.mainTexture = dayTexture;
			}
		}
		else{
			// Set the night sprite
			foreach(Renderer rendererObject in windowImageRenderers){
				rendererObject.material.mainTexture = nightTexture;
			}
		}
	}
	
//	void OnGUI(){
//		if(GUI.Button(new Rect(10f, 10f, 10f, 10f), "1")){
//			setTime(true);
//		}
//		
//		if(GUI.Button(new Rect(30f, 10f, 10f, 10f), "2")){
//			setTime (false);
//		}
//	}
}
