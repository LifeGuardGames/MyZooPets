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
			Debug.Log("NORMAL");
			PlayerShooterController.Instance.isPiercing = false;
			PlayerShooterController.Instance.isTriple = false;
			break;
		case "triple":
			PlayerShooterController.Instance.isTriple = true;
			Debug.Log("TRIPLE");
			StartCoroutine(ResetPowerUP());
			break;
		case "pierce":
			PlayerShooterController.Instance.isPiercing = true;
			Debug.Log("PIERCE");
			StartCoroutine(ResetPowerUP());
			break;
		}
	}

	// Powering down form power up
	private IEnumerator ResetPowerUP(){
		Debug.Log("reaching");
		yield return new WaitForSeconds(timer);
		powerUp = "normal";
		Debug.Log("working");
		PowerUP();
	}
}
