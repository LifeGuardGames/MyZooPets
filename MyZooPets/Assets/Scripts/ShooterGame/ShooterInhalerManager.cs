using UnityEngine;
using System;

public class ShooterInhalerManager :Singleton<ShooterInhalerManager> {
	public EventHandler<EventArgs> proceed;
	public int missed = 0;
	public int combo = 5;


	public void Reset() {
		combo = 5;
		missed = 0;
	}
	
	public void OnShooterGameInhalerButton(){
		// if they can use the inhaler reward them with health and points
			if(ShooterGameManager.Instance.inTutorial==true){
				if(proceed != null){
					proceed(this, EventArgs.Empty);
				}
			}
			combo++;
			if(ShooterGameManager.Instance.highestCombo < combo) {
				ShooterGameManager.Instance.highestCombo = combo;
			}
			ShooterGameManager.Instance.AddScore(10);
			PlayerShooterController.Instance.ChangeHealth(combo);
			AudioManager.Instance.PlayClip("shooterButtonSuccess");
		}
}
