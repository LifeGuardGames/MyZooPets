using UnityEngine;
using System;
using System.Collections;

public class DayNightSwitch : MonoBehaviour{
	
	public bool isDayTime = true;
	public Color cameraDayColor = new Color(167f/255f, 219f/255f, 255f/255f, 1);
	public Color cameraNightColor = new Color(46f/255f, 42f/255f, 92f/255f, 1);
	public Color materialDayTint = new Color(1, 1, 1, 1);
	public Color materialNightTint = new Color(46f/255f, 42f/255f, 92f/255f, 1);
	public Renderer[] rendererList;

	void Start(){
		CheckTime();
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			CheckTime();
		}
	}

	private void CheckTime(){
		DateTime now = LgDateTime.GetTimeNow();
		if(now.Hour	>= 6 && now.Hour < 18){
			ActivateDay();
		}
		else{
			ActivateNight();
		}
	}

	private void ActivateDay(){
		isDayTime = true;
		Camera.main.backgroundColor = cameraDayColor;
		foreach(Renderer ren in rendererList){
			ren.material.color = materialDayTint;
		}
	}
	
	private void ActivateNight(){
		isDayTime = false;
		Camera.main.backgroundColor = cameraNightColor;
		foreach(Renderer ren in rendererList){
			ren.material.color = materialNightTint;
		}
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
