using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterUIManager :Singleton<ShooterUIManager> {
	public GameObject Sun;
	public GameObject Moon;
	public GameObject PosSky;
	public GameObject PosBottom;

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			LeanTween.cancel(Sun);
			LeanTween.cancel(Moon);
			break;
		case MinigameStates.Paused:
			LeanTween.pause(Sun);
			LeanTween.pause(Moon);
			break;
		case MinigameStates.Playing:
			LeanTween.resume(Sun);
			LeanTween.resume(Moon);
			break;
		}
	}
	// Use this for initialization
	void Start () {
		ShooterGameManager.OnStateChanged+=OnGameStateChanged;
	}
	public void reset(){
		Sun.transform.position=PosSky.transform.position;
		Moon.transform.position=PosBottom.transform.position;
	}
	public void AChangeOfTimeActOne(){
		if(ShooterGameManager.Instance.GetGameState()!=MinigameStates.GameOver){
		if(Sun.GetComponent<MovingSky>().InSky==true){
			LeanTween.move(Sun,PosBottom.transform.position,2.0f).setOnComplete(AChangeOfTimeActTwo);
		}
		else{
			LeanTween.move(Moon,PosBottom.transform.position,2.0f).setOnComplete(AChangeOfTimeActTwo);
		}
	}

	}
	public void AChangeOfTimeActTwo(){
		if(Sun.GetComponent<MovingSky>().InSky==true){
			LeanTween.move(Moon,PosSky.transform.position,2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			Moon.GetComponent<MovingSky>().InSky=true;
			Sun.GetComponent<MovingSky>().InSky=false;
		}
		else{
			LeanTween.move(Sun,PosSky.transform.position,2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			Sun.GetComponent<MovingSky>().InSky=true;
			Moon.GetComponent<MovingSky>().InSky=false;
		}
	}

}
