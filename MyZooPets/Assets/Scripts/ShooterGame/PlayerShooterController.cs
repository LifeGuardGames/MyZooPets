using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerShooterController : Singleton<PlayerShooterController>{
	public ShooterCharacterController characterController;
	public EventHandler <EventArgs> changeInHealth;
	public string state = "neutral";	//the player state this dictates the pets strength
	public float playerHealth;			//player health
	public List<GameObject> fireBallPrefabs;	// List of fireball presets too choose from
	public Transform bulletSpawnLocation;		// location that the bullets spawn at aka the mouth not the middle of the chest
	private GameObject currentFireBall;			// our fireball so we can modify it's properties and change its direction
	public bool isTriple;						// are we triple firing
	public bool isPiercing;

	// on reset change health to 10 and state to neutral
	public void Reset(){
		playerHealth = 10;
		ChangeState("neutral");
		this.collider2D.enabled = true;
	}
	// change states
	private void ChangeState(string _state){
		state = _state;
		switch(state){
		case "happy":
			characterController.SetState(ShooterCharacterController.ShooterCharacterStates.happy);
			currentFireBall = fireBallPrefabs[0];	// Big fireball prefab
			break;
		case "neutral":
			characterController.SetState(ShooterCharacterController.ShooterCharacterStates.neutral);
			currentFireBall = fireBallPrefabs[1];	// Medium fireball prefab
			break;
		case "distressed":
			characterController.SetState(ShooterCharacterController.ShooterCharacterStates.distressed);
			currentFireBall = fireBallPrefabs[2];	// Small fireball prefab
			break;
		}
	}

	private string CheckState(){
		return state;
	}

	// removes health and then calculates state
	public void ChangeHealth(float amount){
		if( ShooterGameManager.Instance.GetGameState() != MinigameStates.GameOver){
		
		//being super redundent to fix a game crashing bug
		if( ShooterGameManager.Instance.GetGameState() != MinigameStates.GameOver){
			// Also updates the lives in game manager as that is the true health
			ShooterGameManager.Instance.UpdateLives((int)amount);
			playerHealth = ShooterGameManager.Instance.GetLives();
			if(playerHealth >= 11){
				ChangeState("happy");
			}
			else if(playerHealth > 5 && playerHealth <= 10){
				ChangeState("neutral");
			}
			else if(playerHealth <= 5 && playerHealth > 0){
				ChangeState("distressed");
			}
			else if (playerHealth <= 0){
				this.collider2D.enabled = false;
				characterController.SetState(ShooterCharacterController.ShooterCharacterStates.dead);
			}
		}

		if(amount > 0){ 
			// work around for increaseing health above max 
			if(changeInHealth != null)
				changeInHealth(this, EventArgs.Empty);
		}
		// amount is 0 or less remove the listener
		else{
			changeInHealth -= ShooterGameManager.Instance.HealthUpdate;
		}
		}
	}

	// shoots a bullet at the current position of the mouse or touch
	public void Shoot(Vector3 dir){
		AudioManager.Instance.PlayClip("shooterFire", variations: 3);
		characterController.Shoot();	// Tell the animator to shoot

		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);
		if(isPiercing){
			currentFireBall = fireBallPrefabs[3];
		}
		else{
			ChangeState(CheckState());
		}
		GameObject instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
		ShooterGameBulletScript bulletScript = instance.GetComponent<ShooterGameBulletScript>();
		bulletScript.target = lookPos;
		bulletScript.FindTarget();
		bulletScript.isPierceing = isPiercing;
		if(isTriple){
			instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
			bulletScript = instance.GetComponent<ShooterGameBulletScript>();
			bulletScript.target = new Vector3(lookPos.x, lookPos.y+1, lookPos.z);
			bulletScript.FindTarget();
			bulletScript.isPierceing = isPiercing;

			instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
			bulletScript = instance.GetComponent<ShooterGameBulletScript>();
			bulletScript.target = new Vector3(lookPos.x, lookPos.y-1, lookPos.z);
			bulletScript.FindTarget();
			bulletScript.isPierceing = isPiercing;
		}
	}

	// removes health from player when hit by an enemy bullet // written this way to avoid making a mundane script
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "EnemyBullet"){
			ChangeHealth(-1);
			Destroy(collider.gameObject);
		}
	}
}
