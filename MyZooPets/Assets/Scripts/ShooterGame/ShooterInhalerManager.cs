using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {
	public EventHandler<EventArgs> proceed;
	public bool canUseInhalerButton = true;
	public bool hit = false;
	public GameObject badTiming;
	public GameObject[] goodFX;
	public bool CanUseInhalerButton{
		get{
			return canUseInhalerButton;
		}
		set{
			Debug.Log("setting value " + value);
			canUseInhalerButton=value;
		}
	}


	//on button Tap
	public void ShooterGameInhalerButton(){
		hit = true;
		// if they can use the inhaler reward them with health and points
		if(CanUseInhalerButton == false){
			if(ShooterGameManager.Instance.inTutorial==true){
				if(proceed != null)
					proceed(this, EventArgs.Empty);
			}
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.removeHealth(3);
			CanUseInhalerButton =! CanUseInhalerButton;
			foreach (GameObject boom in goodFX){
				boom.SetActive(true);
			}
		}
		else if(CanUseInhalerButton == true){
			badTiming.SetActive(true);
			StartCoroutine(HoldIt());
		}
	}
	IEnumerator HoldIt(){
		yield return new WaitForSeconds (2.0f);
		badTiming.SetActive(false);
	}
}
