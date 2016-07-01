using UnityEngine;
using System;
using System.Collections;

public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {
	public EventHandler<EventArgs> proceed;
	public bool canUseInhalerButton = true;
	public bool hit = false;
	public Animation badTimingAnim;
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

	public void Reset() {
		combo = 5;
		missed = 0;
	}
	
	public void OnShooterGameInhalerButton(){
		hit = true;
		// if they can use the inhaler reward them with health and points
		if(CanUseInhalerButton == false){
			if(ShooterGameManager.Instance.inTutorial==true){
				if(proceed != null){
					proceed(this, EventArgs.Empty);
				}
			}
			combo++;
			if(ShooterGameManager.Instance.highestCombo < combo) {
				ShooterGameManager.Instance.highestCombo = combo;
			}
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

			AudioManager.Instance.PlayClip("minigameError");

			badTimingAnim.gameObject.SetActive(true);
			badTimingAnim.Play();
		}
	}
}
