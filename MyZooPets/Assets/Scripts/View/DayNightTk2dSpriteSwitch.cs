using UnityEngine;
using System;
using System.Collections;

public class DayNightTk2dSpriteSwitch : MonoBehaviour {

	public bool isDayTime = true;
	public tk2dSprite sprite;
	public string daySpriteName;
	public string nightSpriteName;

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
		sprite.SetSprite(daySpriteName);
	}
	
	private void ActivateNight(){
		isDayTime = false;
		sprite.SetSprite(nightSpriteName);
	}
	
//		void OnGUI(){
//			if(GUI.Button(new Rect(100f, 100f, 100f, 100f), "Day")){
//				ActivateDay();
//			}
//			if(GUI.Button(new Rect(200f, 100f, 100f, 100f), "Night")){
//				ActivateNight();
//			}
//		}
}