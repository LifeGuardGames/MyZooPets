using UnityEngine;
using System.Collections;

public class ShooterPowerUpScript : MonoBehaviour {

	public string powerUp;

	void Update(){
		transform.Translate(Time.deltaTime*-2.0f,0,0);
	}

	void OnTriggerEnter2D(Collider2D collider){
	
		if(collider.gameObject.tag == "bullet"){
			Destroy(collider.gameObject);
			Destroy(this.gameObject);
		}
		else if (collider.gameObject.tag == "Player"){
			ShooterPowerUpManager.Instance.changePowerUp(powerUp);
			Destroy(this.gameObject);
		}
	}
}
