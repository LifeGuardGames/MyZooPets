using UnityEngine;
using System.Collections;
using System;

public class PlayerShooterController : Singleton<PlayerShooterController> {
	public EventHandler <EventArgs> changeInHealth;
	//the player state this dictates the pets strength
	public string State = "neutral";
	//player health
	public float PlayerHealth;
	// the fireball scale
	public float FBallScale;
	//The bar Manager used to retrieve health data from bar usage
	public GameObject bulletSpawn;
	public GameObject bullet;

	public void reset(){
		PlayerHealth = 10;
		ChangeState("neutral");
	}

	// Update is called once per frame
	void Update () {
	}
	private void ChangeState(string _state){
		State = _state;
		switch(State){
		case "happy":{
			FBallScale = 1.5f;
			break;
			}
		case "neutral":{
			FBallScale = 1.0f;
			break;
			}
		case "distressed":{
			FBallScale = 0.5f;
			break;
			}
		}
	}

	public void removeHealth (float amount){
		PlayerHealth += amount;
		if (PlayerHealth >= 11){
			ChangeState("happy");
		}
		else if (PlayerHealth > 5 && PlayerHealth <= 10){
			ChangeState("neutral");
		}
		else if (PlayerHealth <= 5){
			ChangeState("distressed");
		}
		ShooterGameManager.Instance.UpdateLives((int)amount);
		if(amount >0){ 
		// work around for increaseing health above max 
		if(changeInHealth != null)
			changeInHealth(this, EventArgs.Empty);
		}
		else{
			changeInHealth-=ShooterGameManager.Instance.healthUpdate;
		}
	}

	public void shoot(Vector3 dir){
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);
		//fBallScale = Player.GetComponent<Player>().FBallScale;
		GameObject instance = Instantiate(bullet, bulletSpawn.transform.position, bullet.transform.rotation)as GameObject;
		instance.GetComponent<bulletScript>().target = lookPos;
		instance.GetComponent<bulletScript>().FindTarget();
		instance.gameObject.transform.localScale *= FBallScale;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "EnemyBullet"){
			removeHealth(-1);
			Destroy(collider.gameObject);
		}
	}
}
