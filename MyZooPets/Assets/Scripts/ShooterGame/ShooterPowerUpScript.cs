using UnityEngine;
using System.Collections;
using System;

public class ShooterPowerUpScript : MonoBehaviour{

	public string powerUpKey;
	public GameObject sprite;
	public ParticleSystem particleSprite;
	public ParticleSystem particleGet;

	void Update(){
		transform.Translate(Time.deltaTime * -2.0f, 0, 0);
	}

	void OnTriggerEnter2D(Collider2D collider){
	 if(collider.gameObject.tag == "Player"){
			// Parse the powerup
			ShooterPowerUpManager.PowerUpType powerUp = (ShooterPowerUpManager.PowerUpType)Enum.Parse(typeof(ShooterPowerUpManager.PowerUpType), powerUpKey);
			ShooterPowerUpManager.Instance.ChangePowerUp(powerUp);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();

			sprite.SetActive(false);
			particleSprite.Stop();
			particleGet.Play();

			Destroy(this.gameObject, 1f);
		}
	}
}
