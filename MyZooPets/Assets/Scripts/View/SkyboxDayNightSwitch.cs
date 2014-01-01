using UnityEngine;
using System;
using System.Collections;

public class SkyboxDayNightSwitch : MonoBehaviour{
	
	public bool isDayTime = true;
	public Color dayColor = new Color(167f/255f, 219f/255f, 255f/255f);
	public Color nightColor = new Color(46f/255f, 42f/255f, 92f/255f);

	void Start(){
		CheckTime();
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused)
			CheckTime();
	}

	private void CheckTime(){
		DateTime now = LgDateTime.GetTimeNow();
		if(now.Hour	< 12)
			ActivateDay();
		else
			ActivateNight();
	}

	private void ActivateDay(){
		isDayTime = true;
		Camera.main.backgroundColor = dayColor;
	}
	
	private void ActivateNight(){
		isDayTime = false;
		Camera.main.backgroundColor = nightColor;
	}
	
//	void OnGUI(){
//		if(GUI.Button(new Rect(100f, 100f, 100f, 100f), "Day")){
//			ActivateDay();
//		}
//		if(GUI.Button(new Rect(200f, 100f, 100f, 100f), "Night")){
//			ActivateNight();
//		}
//	}
}
