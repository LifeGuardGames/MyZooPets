﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public string name;
	public string spritz;
	public string AiScript;
	public UISprite triggerSprite;
	private GameObject tweeningContentParent;
	//public GameObject NGUIParent;
	public void Initialize(){
		triggerSprite.type = UISprite.Type.Simple;
		triggerSprite.spriteName = spritz;
		triggerSprite.MakePixelPerfect();
		triggerSprite.name = spritz;
		tweeningContentParent = triggerSprite.transform.parent.parent.gameObject;
		Debug.Log(AiScript);
		switch(AiScript){
			case"BasicAI":
				this.gameObject.AddComponent<ShooterBasicEnemyAi>();
				break;
			case "MediumAI":
			Debug.Log("yay!");
				this.gameObject.AddComponent<ShooterMediumEnemyAI>();
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
