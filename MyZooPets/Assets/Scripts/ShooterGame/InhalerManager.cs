using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InhalerManager :Singleton<InhalerManager> {


	private bool justUsed=true;

	public bool JustUsed{
		get{
			return justUsed;
		}
		set{
			justUsed=value;
			Debug.Log(value);
		}
	}

	//on button Tap
	public void ClickIt(){
		if(JustUsed==false){
			Debug.Log("b");
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.removeHealth(3);
			JustUsed=!JustUsed;
		}
		else if(JustUsed==true){
			Debug.Log("anasegs");
		}
	}


}
