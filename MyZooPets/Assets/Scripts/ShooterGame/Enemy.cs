using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour{

	public string name;
	public string spritz;
	public string AiScript;
	public UISprite triggerSprite;
	public GameObject NGUIParent;

	public Enemy(){
	}
	public void Initialize(Enemy enemy){
		triggerSprite.type = UISprite.Type.Simple;
		triggerSprite.spriteName = enemy.spritz;
		triggerSprite.MakePixelPerfect();
		triggerSprite.name = enemy.spritz;
		this.gameObject.AddComponent(AiScript);
	}

	/*// Use this for initialization
	void Start () {
	
		Player= GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-Speed*Time.deltaTime,0,0);
	}*/
	/*// when offscreen kill enemy
	void OnBecameInvisible()
	{
		Player.GetComponent<Player>().removeHealth(-damage);
		Destroy(this.gameObject);
	}*/
	// if we hit a bullet destroy both the enemy anbd bullet
	/*void OnTriggerEnter2D(Collider2D Col){
		if(Col.gameObject.tag=="bullet")
		{
			//Player.GetComponent<Player>().AddScore(ScoreVal);
			Destroy(Col.gameObject);
			Destroy(this.gameObject);
		}
		else if (Col.gameObject.tag=="Player"){
			//Player.GetComponent<Player>().removeHealth(-Damage);
			Destroy(this.gameObject);
		}
	}*/
}
