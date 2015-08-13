using UnityEngine;
using System.Collections;
using System;

public class ShooterPowerUpScript : MonoBehaviour{

	public string powerUpKey;

	void Update(){
		transform.Translate(Time.deltaTime * -2.0f, 0, 0);
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "bullet"){
			Destroy(collider.gameObject);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			Destroy(this.gameObject);
		}
		else if(collider.gameObject.tag == "Player"){
			// Parse the powerup
			ShooterPowerUpManager.PowerUpType powerUp = (ShooterPowerUpManager.PowerUpType)Enum.Parse(typeof(ShooterPowerUpManager.PowerUpType), powerUpKey);
			ShooterPowerUpManager.Instance.ChangePowerUp(powerUp);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			Destroy(this.gameObject);
		}
	}
}
