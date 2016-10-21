using UnityEngine;
using System;

public class DayNightSwitch : MonoBehaviour{
	public bool isDayTime = true;
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
		foreach(Renderer ren in rendererList){
			ren.material.color = materialDayTint;
		}
	}
	
	private void ActivateNight(){
		isDayTime = false;
		foreach(Renderer ren in rendererList){
			ren.material.color = materialNightTint;
		}
	}
}
