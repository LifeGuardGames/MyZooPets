using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public string name;
	public string spriteName;
	public string aiScript;
	public UISprite triggerSprite;
	public GameObject bulletPrefab;
	private GameObject tweeningContentParent;
	//public GameObject NGUIParent;

	// initalizes the enemy with the correct values and scripts
	public void Initialize(){
		triggerSprite.type = UISprite.Type.Simple;
		triggerSprite.spriteName = spriteName;
		triggerSprite.MakePixelPerfect();
		triggerSprite.name = spriteName;
		tweeningContentParent = triggerSprite.transform.parent.parent.gameObject;

		// adds the correct script based off the script ai string
		switch(aiScript){
			case"BasicAI":
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
			case "MediumAI":
				this.gameObject.AddComponent<ShooterMediumEnemyAi
				>();
				break;
			case "HardAI":
				this.gameObject.AddComponent<ShooterHardEnemyAi>();
				break;
			default:
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
		}

	}
}
