using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health=1;
	public int damage=1;
	private GameObject Player;

	// Use this for initialization
	void Start () {
	
		Player= GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Time.deltaTime, 0, 0, Camera.main.transform);
	}
	// when offscreen kill enemy
	void OnBecameInvisible()
	{
		Player.GetComponent<ShooterGameManager>().removeHealth(damage);
		Destroy(this.gameObject);
	}
	// if we hit a bullet destroy both the enemy anbd bullet
	void OnCollisionEnter2D(Collision2D collider)
	{
		if(collider.gameObject.tag=="bullet")
		{
			Destroy(collider.gameObject);
			Destroy(this.gameObject);
		}
	}
}
