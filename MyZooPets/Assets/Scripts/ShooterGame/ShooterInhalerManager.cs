using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {
	public EventHandler<EventArgs> proceed;
	public bool canUseInhalerButton = true;
	public bool hit = false;
	public GameObject badTimingObject;
	public ParticleSystem goodTimingParticle;
	public int missed = 0;
	public bool CanUseInhalerButton{
		get{
			return canUseInhalerButton;
		}
		set{
			canUseInhalerButton = value;
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
			if(ShooterUIManager.Instance.fingerPos != null){
				Destroy(ShooterUIManager.Instance.fingerPos.gameObject);
			}
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.RemoveHealth(3);
			CanUseInhalerButton =! CanUseInhalerButton;
			goodTimingParticle.Play();

			AudioManager.Instance.PlayClip("shooterButtonSuccess");
		}
		else if(CanUseInhalerButton == true){
			missed++;
			Debug.Log(badTimingObject.name);
			badTimingObject.SetActive(true);

			AudioManager.Instance.PlayClip("minigameError");
			StartCoroutine(DeactivateText());
		}
	}

	IEnumerator DeactivateText(){
		yield return new WaitForSeconds (2.0f);
		badTimingObject.SetActive(false);
	}
}
