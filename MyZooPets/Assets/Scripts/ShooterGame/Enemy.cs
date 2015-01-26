using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public string name;
	public string spriteName;
	public string aiScript;
	public UISprite triggerSprite;
	private GameObject tweeningContentParent;
	//public GameObject NGUIParent;
	public void Initialize(){
		triggerSprite.type = UISprite.Type.Simple;
		triggerSprite.spriteName = spriteName;
		triggerSprite.MakePixelPerfect();
		triggerSprite.name = spriteName;
		tweeningContentParent = triggerSprite.transform.parent.parent.gameObject;

		switch(aiScript){
			case"BasicAI":
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
			case "MediumAI":
				this.gameObject.AddComponent<ShooterMediumEnemyAi
				>();
				break;
			case "HardAI":
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
			default:
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
		}

	}
}
