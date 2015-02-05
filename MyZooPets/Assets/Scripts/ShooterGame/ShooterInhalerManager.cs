using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {
	public EventHandler<EventArgs> proceed;
	public bool canUseInhalerButton = true;
	public GameObject tooEarly;
	public GameObject tooLate;
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
		// if they can use the inhaler reward them with health and points
		if(CanUseInhalerButton == false){
			if(ShooterGameManager.Instance.inTutorial==true){
				if(proceed != null)
					proceed(this, EventArgs.Empty);
			}
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.removeHealth(3);
			CanUseInhalerButton =! CanUseInhalerButton;
		}
		else if(CanUseInhalerButton == true){
		}
	}
}
