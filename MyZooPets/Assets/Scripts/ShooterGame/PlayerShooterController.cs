using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerShooterController : Singleton<PlayerShooterController>{
	public EventHandler <EventArgs> changeInHealth;
	public string state = "neutral";	//the player state this dictates the pets strength
	public float playerHealth;			//player health
	public List<GameObject> fireBallPrefabs;	// List of fireball presets too choose from
	public Transform bulletSpawnLocation;		// location that the bullets spawn at aka the mouth not the middle of the chest
	private GameObject currentFireBall;			// our fireball so we can modify it's properties and change its direction

	// on reset change health to 10 and state ti neutral
	public void reset(){
		playerHealth = 10;
		ChangeState("neutral");
	}
	// change states
	private void ChangeState(string _state){
		state = _state;
		switch(state){
		case "happy":
			currentFireBall = fireBallPrefabs[0];	// Big fireball prefab
			break;
		case "neutral":
			currentFireBall = fireBallPrefabs[1];	// Medium fireball prefab
			break;
		case "distressed":
			currentFireBall = fireBallPrefabs[2];	// Small fireball prefab
			break;
		}
	}

	// removes health and then calculates state
	public void removeHealth(float amount){
		playerHealth += amount;
		if(playerHealth >= 11){
			ChangeState("happy");
		}
		else if(playerHealth > 5 && playerHealth <= 10){
			ChangeState("neutral");
		}
		else if(playerHealth <= 5){
			ChangeState("distressed");
		}
		// Also updates the lives in game manager as that is the true health
		ShooterGameManager.Instance.UpdateLives((int)amount);
		if(amount > 0){ 
			// work around for increaseing health above max 
			if(changeInHealth != null)
				changeInHealth(this, EventArgs.Empty);
		}
		// amount is 0 or less remove the listener
		else{
			changeInHealth -= ShooterGameManager.Instance.healthUpdate;
		}
	}

	// shoots a bullet at the current position of the mouse or touch
	public void shoot(Vector3 dir){
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);
		GameObject instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation)as GameObject;
		instance.GetComponent<bulletScript>().target = lookPos;
		instance.GetComponent<bulletScript>().FindTarget();
	}

	// removes health from player when hit by an enemy bullet // written this way to avoid making a mundane script
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "EnemyBullet"){
			removeHealth(-1);
			Destroy(collider.gameObject);
		}
	}
}
