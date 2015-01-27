using UnityEngine;
using System.Collections;

public class ShooterEnemyAi : MonoBehaviour{
	public float speed;
	public int scoreVal;
	public int damage;
	public int health;
	protected GameObject player;

	// Use this for initialization
	void Awake(){
		player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}

	// Update is called once per frame
	void Update(){
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.Paused){
			LeanTween.pause(this.gameObject);
		}
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.GameOver){
			StartCoroutine(DestroyEnemy());
		}
	}

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			StartCoroutine(DestroyEnemy());
			break;
		case MinigameStates.Paused:
			LeanTween.pause(this.gameObject);
			break;
		case MinigameStates.Playing:
			LeanTween.resume(this.gameObject);
			break;
		case MinigameStates.Restarting:
			StartCoroutine(DestroyEnemy());
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "bullet"){
			
			Destroy(collider.gameObject);
			health--;
			if(health <= 0){
				ShooterGameManager.Instance.AddScore(scoreVal);
				StartCoroutine(DestroyEnemy());
			}
		}
		else if(collider.gameObject.tag == "Player"){
			PlayerShooterController.Instance.removeHealth(-damage);
			StartCoroutine(DestroyEnemy());
		}
	}
	
	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		LeanTween.cancel(this.gameObject);
		ShooterGameEnemyController.Instance.enemiesInWave--;
		ShooterGameEnemyController.Instance.CheckEnemiesInWave();
		ShooterGameManager.OnStateChanged -= OnGameStateChanged;
		Destroy(this.gameObject);
	}
}
