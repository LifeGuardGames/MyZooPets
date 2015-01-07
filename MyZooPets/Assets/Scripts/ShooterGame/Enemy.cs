using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health=1;
	public int damage=1;
	public float speed=5;
	private GameObject Player;


	// Use this for initialization
	void Start () {
	
		Player= GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-speed*Time.deltaTime,0,0);
	}
	// when offscreen kill enemy
	void OnBecameInvisible()
	{
		Player.GetComponent<Player>().removeHealth(-damage);
		Destroy(this.gameObject);
	}
	// if we hit a bullet destroy both the enemy anbd bullet
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag=="bullet")
		{
			Destroy(collider.gameObject);
			Destroy(this.gameObject);
		}
		if (collider.gameObject.tag=="Player")
		{
			Player.GetComponent<Player>().removeHealth(-damage);
			Destroy(this.gameObject);
		}
	}
}
