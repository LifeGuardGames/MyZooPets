using UnityEngine;
using System.Collections;

public class ShooterPowerUpScript : MonoBehaviour {

	public string powerUp;
	public string name;
	public string spriteName;
	public string aiScript;
	public UISprite powerSprite;
	private GameObject tweeningContentParent;
	
	// initalizes the enemy with the correct values and scripts
	public void Initialize(){
		//powerSprite.type = UISprite.Type.Simple;
		//powerSprite.spriteName = spriteName;
		//powerSprite.MakePixelPerfect();
		//powerSprite.name = spriteName;
		//tweeningContentParent = powerSprite.transform.parent.parent.gameObject;
		Debug.Log(aiScript);
		powerUp = aiScript;
	}

	void Update(){
		transform.Translate(Time.deltaTime*-2.0f,0,0);
	}

	void OnTriggerEnter2D(Collider2D collider){
	
		if(collider.gameObject.tag == "bullet"){
			Destroy(collider.gameObject);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			Destroy(this.gameObject);
		}
		else if (collider.gameObject.tag == "Player"){
			ShooterPowerUpManager.Instance.ChangePowerUp(powerUp);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			Destroy(this.gameObject);
		}
	}
}
