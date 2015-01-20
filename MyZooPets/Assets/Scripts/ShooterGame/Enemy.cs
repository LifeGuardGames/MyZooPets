using UnityEngine;
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
		this.gameObject.AddComponent<BasicEnemyAi>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
