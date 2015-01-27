using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterUIManager :Singleton<ShooterUIManager>{
	public GameObject sun;
	public GameObject moon;
	public Transform posSky;
	public Transform posBottom;

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			LeanTween.cancel(sun);
			LeanTween.cancel(moon);
			break;
		case MinigameStates.Paused:
			LeanTween.pause(sun);
			LeanTween.pause(moon);
			break;
		case MinigameStates.Playing:
			LeanTween.resume(sun);
			LeanTween.resume(moon);
			break;
		case MinigameStates.Restarting:
			LeanTween.cancel(sun);
			LeanTween.cancel(moon);
			break;
		}
	}

	// Use this for initialization
	void Start(){
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}

	public void Reset(){
		sun.transform.position = posSky.position;
		moon.transform.position = posBottom.position;
	}

	public void AChangeOfTimeActOne(){
		if(!ShooterGameManager.Instance.inTutorial){
			if(ShooterGameManager.Instance.GetGameState() != MinigameStates.GameOver){
				if(sun.GetComponent<MovingSky>().inSky == true){
					LeanTween.move(sun, posBottom.position, 2.0f).setOnComplete(AChangeOfTimeActTwo);
				}
				else{
					LeanTween.move(moon, posBottom.position, 2.0f).setOnComplete(AChangeOfTimeActTwo);
				}
			}
		}
		else{
			LeanTween.move(sun, posBottom.position, 2.0f);
		}
	}

	public void AChangeOfTimeActTwo(){
		MovingSky sunScript = sun.GetComponent<MovingSky>();
		MovingSky moonScript = moon.GetComponent<MovingSky>();

		if(sunScript.inSky == true){
			LeanTween.move(moon, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			moonScript.inSky = true;
			sunScript.inSky = false;
		}
		else{
			LeanTween.move(sun, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			sunScript.inSky = true;
			moonScript.inSky = false;
		}
	}
}
