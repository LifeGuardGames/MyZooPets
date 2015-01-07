using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int Health=1;
	public int Damage=1;
	public float Speed=5;
	public int ScoreVal=1;
	private GameObject Player;


	// Use this for initialization
	void Start () {
	
		Player= GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-Speed*Time.deltaTime,0,0);
	}
	/*// when offscreen kill enemy
	void OnBecameInvisible()
	{
		Player.GetComponent<Player>().removeHealth(-damage);
		Destroy(this.gameObject);
	}*/
	// if we hit a bullet destroy both the enemy anbd bullet
	void OnTriggerEnter2D(Collider2D Col){
		if(Col.gameObject.tag=="bullet")
		{
			Player.GetComponent<Player>().AddScore(ScoreVal);
			Destroy(Col.gameObject);
			Destroy(this.gameObject);
		}
		else if (Col.gameObject.tag=="Player"){
			Player.GetComponent<Player>().removeHealth(-Damage);
			Destroy(this.gameObject);
		}
	}
}
