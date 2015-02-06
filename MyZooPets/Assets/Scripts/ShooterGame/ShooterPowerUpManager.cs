using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager> {
	private string powerUp = "normal";
	public float timer;
	public void changePowerUp(string _powerUp){
		powerUp = _powerUp;
	}

	public void powerUP(GameObject instance){
		switch(powerUp){
		case "normal":
			instance.GetComponent<bulletScript>().isPierceing = false;
			break;
		case "tripple":
			// TODO add powerupscript here
			break;
		case "pierce":
			instance.GetComponent<bulletScript>().isPierceing = true;
			StartCoroutine("powerTime", instance);
			break;
		}
	}
	IEnumerator powerTime(GameObject _instance){
		yield return new WaitForSeconds(timer);
		powerUp = "normal";
		powerUP(_instance);
	}
}
