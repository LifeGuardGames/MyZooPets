using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE: This script keeps the internal health of the actual pet, instead of using lives since it fluctuates a lot
/// </summary>
public class PlayerShooterController : Singleton<PlayerShooterController> {
	public enum PlayerStateTypes {
		Happy,
		Neutral,
		Distressed
	}

	public int playerHealth = 5;                // player health
	public EventHandler<EventArgs> OnTutorialMove;
	private PlayerStateTypes playerState = PlayerStateTypes.Neutral;
	public PlayerStateTypes PlayerState {
		get { return playerState; }
	}

	public ShooterCharacterAnimController characterAnim;
	public List<GameObject> fireBallPrefabs;    // List of fireball presets too choose from
	public Transform bulletSpawnLocation;       // location that the bullets spawn at aka the mouth not the middle of the chest
	private GameObject currentFireBall;         // our fireball so we can modify it's properties and change its direction
	public float moveSpeed = 10;
	public Vector3 clickPos;
	public bool moving = false;
	public ParticleSystem flameUpParticle;
	public ParticleSystem powerUpParticle;

	public bool isTriple;                       // Triple firing upgrade
	public bool IsTriple {
		get { return isTriple; }
		set { isTriple = value; }
	}

	public bool isPiercing;                     // Piercing upgrade
	public bool IsPiercing {
		get { return isPiercing; }
		set { isPiercing = value; }
	}

	void Start() {
		Reset();
	}

	// on reset change health to 10 and state to neutral
	public void Reset() {
		if(ShooterGameManager.Instance.mode == MiniGameModes.Life) {
			playerHealth = 1;
		}
		else {
			playerHealth = 5;
		}
		ChangeState(PlayerStateTypes.Neutral);
		ChangeFire();
		this.GetComponent<Collider2D>().enabled = true;
	}

	// change states
	public void ChangeState(PlayerStateTypes state) {
		playerState = state;
		switch(state) {
			case PlayerStateTypes.Happy:
				characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Happy);
				break;
			case PlayerStateTypes.Neutral:
				characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Neutral);
				break;
			case PlayerStateTypes.Distressed:
				characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Distressed);
				break;
		}
	}

	public void ChangeFire() {
		switch(playerHealth) {
			case 1:
				currentFireBall = fireBallPrefabs[0];
				break;
			case 2:
				currentFireBall = fireBallPrefabs[0];
				break;
			case 3:
				currentFireBall = fireBallPrefabs[0];
				break;
			case 4:
				currentFireBall = fireBallPrefabs[2];
				break;
			case 5:
				currentFireBall = fireBallPrefabs[2];
				break;
			case 6:
				currentFireBall = fireBallPrefabs[3];
				break;
			case 7:
				currentFireBall = fireBallPrefabs[3];
				break;
			case 8:
				currentFireBall = fireBallPrefabs[4];
				break;
			case 9:
				currentFireBall = fireBallPrefabs[4];
				break;
			case 10:
				currentFireBall = fireBallPrefabs[5];
				break;
			case 11:
				currentFireBall = fireBallPrefabs[5];
				break;
			case 12:
				currentFireBall = fireBallPrefabs[6];
				break;
			case 13:
				currentFireBall = fireBallPrefabs[6];
				isPiercing = false;
				break;
			case 14:
				currentFireBall = fireBallPrefabs[6];
				isPiercing = true;
				break;
		}
	}

	public void PlayPowerUpEffects() {
		AudioManager.Instance.PlayClip("shooterPowerUp");
		powerUpParticle.Play();
    }

	public void PlayFlameUpEffects() {
		AudioManager.Instance.PlayClip("shooterFlameUp");
		flameUpParticle.Play();
	}

	// removes health and then calculates state
	public void ChangeHealth(int deltaHealth) {
		if(!ShooterGameManager.Instance.isGameOver) {
			if(deltaHealth < 0) {
				AudioManager.Instance.PlayClip("shooterHurt");
				playerHealth += deltaHealth;
				BloodPanelManager.Instance.PlayBlood();
			}
			else {
				if(playerHealth < deltaHealth && deltaHealth < 14 ) {
					playerHealth = deltaHealth;
				}
				if(ShooterGameManager.Instance.mode == MiniGameModes.Life) {
					playerHealth = 1;
				}
			}

			if(playerHealth >= 4) {
				ChangeState(PlayerStateTypes.Happy);
			}
			else if(playerHealth > 1 && playerHealth <= 3) {
				ChangeState(PlayerStateTypes.Neutral);
			}
			else if(playerHealth <= 1 && playerHealth > 0) {
				ChangeState(PlayerStateTypes.Distressed);
			}

			if(deltaHealth > 14) {
				playerHealth = 14;  // Cap health at 15
			}

			if(playerHealth <= 0) {
				this.GetComponent<Collider2D>().enabled = false;
				characterAnim.SetState(ShooterCharacterAnimController.ShooterCharacterStates.Dead);
				// Trigger game over
				ShooterGameManager.Instance.GameOver();
			}
			else {
				ChangeFire();
			}
		}
		else {
			Debug.LogError("Trying to change health after game over");
		}
	}

	public void Move(Vector3 dir) {
		if(!ShooterGameManager.Instance.IsPaused) {
			if(ShooterGameManager.Instance.inTutorial) {
				if(OnTutorialMove != null) {
					OnTutorialMove(this, EventArgs.Empty);
				}
			}
			clickPos = dir;
			moving = true;
		}
	}

	void FixedUpdate() {
		if(moving && !ShooterGameManager.Instance.IsPaused) {
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(clickPos).y, transform.position.z), moveSpeed * Time.deltaTime);
		}
	}

	// shoots a bullet at the current position of the mouse or touch
	public void Shoot(Vector3 dir) {
		if(!ShooterGameManager.Instance.IsPaused) {
			AudioManager.Instance.PlayClip("shooterFire", variations: 3);
			characterAnim.Shoot();  // Tell the animator to shoot

			Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);


			GameObject instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
			ShooterGameBulletScript bulletScript = instance.GetComponent<ShooterGameBulletScript>();
			bulletScript.target = lookPos;
			bulletScript.FindTarget();
			if(isTriple) {
				instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
				bulletScript = instance.GetComponent<ShooterGameBulletScript>();
				bulletScript.target = new Vector3(lookPos.x, lookPos.y + 1, lookPos.z);
				bulletScript.FindTarget();
				bulletScript.isPierceing = isPiercing;

				instance = Instantiate(currentFireBall, bulletSpawnLocation.transform.position, currentFireBall.transform.rotation) as GameObject;
				bulletScript = instance.GetComponent<ShooterGameBulletScript>();
				bulletScript.target = new Vector3(lookPos.x, lookPos.y - 1, lookPos.z);
				bulletScript.FindTarget();
			}
		}
	}

	// removes health from player when hit by an enemy smog ball // written this way to avoid making a mundane script
	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.gameObject.tag == "ShooterEnemyBullet") {
			ChangeHealth(-1);
			Destroy(collider.gameObject);
		}
	}
}
