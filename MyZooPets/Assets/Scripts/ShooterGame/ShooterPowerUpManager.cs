using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager>{
	private string powerUp = "normal";
	public float timer;

	public void ChangePowerUp(string _powerUp){
		powerUp = _powerUp;
		PowerUP();
	}

	public void PowerUP(){
		switch(powerUp){
		case "normal":

			PlayerShooterController.Instance.isPiercing = false;
			PlayerShooterController.Instance.isTriple = false;
			break;
		case "triple":
			PlayerShooterController.Instance.isTriple = true;

			StartCoroutine(ResetPowerUP());
			break;
		case "pierce":
			PlayerShooterController.Instance.isPiercing = true;

			StartCoroutine(ResetPowerUP());
			break;
		}
	}

	// Powering down form power up
	private IEnumerator ResetPowerUP(){
		yield return new WaitForSeconds(timer);
		powerUp = "normal";
		PowerUP();
	}
}
