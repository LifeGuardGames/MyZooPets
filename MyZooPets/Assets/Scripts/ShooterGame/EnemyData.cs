using UnityEngine;
using System.Collections;

public class EnemyData{

	public string name;
	public string spriteName;
	public string aiScript;
	public UISprite triggerSprite;

	public EnemyData(){
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
