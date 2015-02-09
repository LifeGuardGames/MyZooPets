using UnityEngine;
using System.Collections;

public class ShooterPowerUpInit : MonoBehaviour {

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
		this.gameObject.GetComponent<ShooterPowerUpScript>().powerUp = aiScript;
	}
}
