using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {

	public EventHandler<EventArgs> Proceed;
	public bool canUseInhalerButton = true;

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
		if(CanUseInhalerButton == false){
			if(ShooterGameManager.Instance.InTutorial==true){
					if(Proceed != null)
						Proceed(this, EventArgs.Empty);
			}
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.removeHealth(3);
			CanUseInhalerButton =! CanUseInhalerButton;
		}
		else if(CanUseInhalerButton == true){
			Debug.Log("anasegs");
		}
	}
}
