using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShooterUIManager : MinigameUI{

	public GameObject sun;
	public GameObject moon;
	public Transform posSky;
	public Transform posBottom;
	public TextureListAlphaTween dayTween;
	public TextureListAlphaTween nightTween;
	public GameObject fingerPos;
	public GameObject FingerPos{
		get{ return fingerPos; }
	}

	// handles the game state changes
	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			if(sun != null)
				LeanTween.cancel(sun);
			if(moon != null)
				LeanTween.cancel(moon);
			break;
		case MinigameStates.Paused:
			if(sun != null)
				LeanTween.pause(sun);
			if(moon != null)
				LeanTween.pause(moon);
			break;
		case MinigameStates.Playing:
			if(sun != null)
				LeanTween.resume(sun);
			if(moon != null)
				LeanTween.resume(moon);
			break;
		case MinigameStates.Restarting:
			if(sun != null)
				LeanTween.cancel(sun);
			if(moon != null)
				LeanTween.cancel(moon);
			break;
		}
	}

	public void Quit(){
		LeanTween.cancel(sun);
		LeanTween.cancel(moon);
	}

	// Use this for initialization
	void Start(){
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}

	public void Reset(){
		sun = GameObject.Find("SpriteSun");
		moon = GameObject.Find("SpriteMoon");
		sun.transform.position = posSky.position;
		sun.GetComponent<MovingSky>().inSky = true;
		moon.GetComponent<MovingSky>().inSky = false;
		moon.transform.position = posBottom.position;
		dayTween.InstantShow();
		nightTween.InstantHide();
	}

	// changes the sun to moon or moon to sun and then sets off the next transition once it is complete
	public void StartTimeTransition(){
		if(!ShooterGameManager.Instance.inTutorial){
			if(ShooterGameManager.Instance.GetGameState() != MinigameStates.GameOver){
				if(ShooterGameManager.Instance.waveNum == 0){
					GameObject tutorialFinger =  (GameObject)Resources.Load("ShooterPressTut");
					fingerPos = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-BottomRight"),tutorialFinger);
				}
				if(sun.GetComponent<MovingSky>().inSky == true){
					LeanTween.move(sun, posBottom.position, 2.0f).setOnComplete(EndTimeTransition).setEase(LeanTweenType.easeInQuad);
					nightTween.Show();
					dayTween.Hide();
				}
				else{
					LeanTween.move(moon, posBottom.position, 2.0f).setOnComplete(EndTimeTransition).setEase(LeanTweenType.easeInQuad);
					dayTween.Show();
					nightTween.Hide();
				}
			}
		}
		else{
			LeanTween.move(sun, posBottom.position, 2.0f).setOnComplete(TutChange).setEase(LeanTweenType.easeInQuad);
			nightTween.Show();
			dayTween.Hide();
		}
	}

	public void TutChange(){
		// if its the tutorial go to next step
		LeanTween.move(moon, posSky.position, 2.0f).setEase(LeanTweenType.easeOutQuad);
	}

	// Finishes the time transition and starts new wave
	private void EndTimeTransition(){
		MovingSky sunScript = sun.GetComponent<MovingSky>();
		MovingSky moonScript = moon.GetComponent<MovingSky>();

		if(sunScript.inSky == true){
			LeanTween.move(moon, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.BeginNewWave).setEase(LeanTweenType.easeOutQuad);
			moonScript.inSky = true;
			sunScript.inSky = false;
		}
		else{
			LeanTween.move(sun, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.BeginNewWave).setEase(LeanTweenType.easeOutQuad);
			sunScript.inSky = true;
			moonScript.inSky = false;
		}
	}
}
