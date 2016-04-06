using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// NOTE: This script keeps the internal health of the actual pet, instead of using lives since it fluctuates a lot
/// </summary>
public class PlayerShooterController : Singleton<PlayerShooterController>{
	public enum PlayerStateTypes{
		Happy,
		Neutral,
		Distressed
	}

	public int playerHealth = 10;				// player health

	private PlayerStateTypes playerState = PlayerStateTypes.Neutral;
	public PlayerStateTypes PlayerState{
		get{ return playerState; }
	}

	public ShooterCharacterAnimController characterAnim;
	public List<GameObject> fireBallPrefabs;	// List of fireball presets too choose from
	public Transform bulletSpawnLocation;		// location that the bullets spawn at aka the mouth not the middle of the chest
	private GameObject currentFireBall;			// our fireball so we can modify it's properties and change its direction

	public bool isTriple;						// Triple firing upgrade
	public bool IsTriple{
		get{ return isTriple; }
		set{ isTriple = value; }
	}

	public bool isPiercing;						// Piercing upgrade
	public bool IsPiercing{
		get{ return isPiercing; }
		set{ isPiercing = value; }
	}

	// on reset change health to 10 and state to neutral
	public void Reset(){
		playerHealth = 10;
		ChangeState(PlayerStateTypes.Neutral);
		this.collider2D.enabled = true;
	}

	// change states
	public void ChangeState(PlayerStateTypes state){
		playerState = state;
		switch(state){
		case PlayerStateTypes.Happy:
			characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Happy);
			currentFireBall = fireBallPrefabs[0];	// Big fireball prefab
			break;
		case PlayerStateTypes.Neutral:
			characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Neutral);
			currentFireBall = fireBallPrefabs[1];	// Medium fireball prefab
			break;
		case PlayerStateTypes.Distressed:
			characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Distressed);
			currentFireBall = fireBallPrefabs[2];	// Small fireball prefab
			break;
		}
	}

	// removes health and then calculates state
	public void ChangeHealth(int deltaHealth){
		if(ShooterGameManager.Instance.GetGameState() != MinigameStates.GameOver){
			if(deltaHealth < 0){
				AudioManager.Instance.PlayClip("shooterHurt");
			}

			playerHealth += deltaHealth;
			if(playerHealth >= 11){
				if(playerHealth > 15){
					playerHealth = 15;	// Cap health at 15
				}
				ChangeState(PlayerStateTypes.Happy);
			}
			else if(playerHealth > 5 && playerHealth <= 10){
				ChangeState(PlayerStateTypes.Neutral);
			}
			else if(playerHealth <= 5 && playerHealth > 0){
				ChangeState(PlayerStateTypes.Distressed);
			}
			else if(playerHealth <= 0){
				this.collider2D.enabled = false;
				characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Dead);
			}

			if(playerHealth <= 0){
				// Trigger game over
				ShooterGameManager.Instance.UpdateLives(-1);
			}
		}
		else{
			Debug.LogError("Trying to change health after game over");
		}
	}

	public void Move(Vector3 dir){
		LeanTween.moveY(this.gameObject, Camera.main.ScreenToWorldPoint(dir).y, 1.0f);
	}

	// shoots a bullet at the current position of the mouse or touch
	public void Shoot(Vector3 dir){
		AudioManager.Instance.PlayClip("shooterFire", variations: 3);
		characterAnim.Shoot();	// Tell the animator to shoot

		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);

		if(isPiercing){
			currentFireBall = fireBallPrefabs[3];
		}

		GameObject instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
		ShooterGameBulletScript bulletScript = instance.GetComponent<ShooterGameBulletScript>();
		bulletScript.target = lookPos;
		bulletScript.FindTarget();
		bulletScript.isPierceing = isPiercing;
		if(isTriple){
			instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
			bulletScript = instance.GetComponent<ShooterGameBulletScript>();
			bulletScript.target = new Vector3(lookPos.x, lookPos.y + 1, lookPos.z);
			bulletScript.FindTarget();
			bulletScript.isPierceing = isPiercing;

			instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
			bulletScript = instance.GetComponent<ShooterGameBulletScript>();
			bulletScript.target = new Vector3(lookPos.x, lookPos.y - 1, lookPos.z);
			bulletScript.FindTarget();
			bulletScript.isPierceing = isPiercing;
		}
	}

	// removes health from player when hit by an enemy smog ball // written this way to avoid making a mundane script
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "EnemyBullet"){
			ChangeHealth(-1);
			Destroy(collider.gameObject);
		}
	}
}
