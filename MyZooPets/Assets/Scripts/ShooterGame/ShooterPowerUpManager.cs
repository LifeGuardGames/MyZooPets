using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager> {
	private string powerUp = "normal";
	public float timer;
	public void changePowerUp(string _powerUp){
		powerUp = _powerUp;
		powerUP();
	}

	public void powerUP(){
		switch(powerUp){
		case "normal":
			PlayerShooterController.Instance.isPiercing = false;
			PlayerShooterController.Instance.isTripple = false;
			break;
		case "tripple":
			PlayerShooterController.Instance.isTripple = true;
			StartCoroutine("powerTime");
			break;
		case "pierce":
			PlayerShooterController.Instance.isPiercing = true;
			StartCoroutine("powerTime");
			break;
		}
	}
	IEnumerator powerTime(){
		Debug.Log("reaching");
		yield return new WaitForSeconds(timer);
		powerUp = "normal";
		Debug.Log("working");
		powerUP();
	}
	public void GetPowerUp(){
	
	}
}
