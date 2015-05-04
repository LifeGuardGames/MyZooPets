using UnityEngine;
using System.Collections;

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
			ShooterPowerUpManager.Instance.ChangePowerUp(powerUpKey);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			Destroy(this.gameObject);
		}
		else{
			Debug.LogWarning("Foreign object collision deteted " + collider.name);
		}
	}
}
