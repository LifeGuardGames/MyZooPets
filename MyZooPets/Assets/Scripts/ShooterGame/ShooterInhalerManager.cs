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
	public int combo = 5;
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
				if(proceed != null){
					proceed(this, EventArgs.Empty);
				}
			}
			combo++;
			ShooterGameManager.Instance.RemoveInhalerFingerTutorial();
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.ChangeHealth(combo);
			CanUseInhalerButton =! CanUseInhalerButton;
			goodTimingParticle.Play();
			AudioManager.Instance.PlayClip("shooterButtonSuccess");
		}
		else if(CanUseInhalerButton == true){
			combo = 0;
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
