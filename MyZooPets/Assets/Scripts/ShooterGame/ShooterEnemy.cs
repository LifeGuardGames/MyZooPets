using UnityEngine;
using System.Collections;

public class ShooterEnemy : MonoBehaviour{
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
		// work around for enemies who spawn during a state change they seem to miss the event call when this happens
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.Paused){
			LeanTween.pause(this.gameObject);
		}
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.GameOver){
			StartCoroutine(DestroyEnemy());
		}
	}
	
	private void OnDisable(){
		LeanTween.cancel(this.gameObject);
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
			if(this.gameObject != null){
			LeanTween.resume(this.gameObject);
			}
			break;
		case MinigameStates.Restarting:
			StartCoroutine(DestroyEnemy());
			break;
		}
	}

	// handles collision not too much special there
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "bullet"){
			if(!collider.GetComponent<BulletScript>().isPierceing){
				Destroy(collider.gameObject);
			}
			health--;
			if(health <= 0){
				ShooterGameManager.Instance.AddScore(scoreVal);
				StartCoroutine(DestroyEnemy());
			}
		}
		else if(collider.gameObject.tag == "Player"){
			PlayerShooterController.Instance.RemoveHealth(-damage);
			StartCoroutine(DestroyEnemy());
		}
	}

	// this is a corutine to make sure enemies are destroyed at the end of frame otherwise an error is thrown by NGUI
	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		LeanTween.cancel(this.gameObject);
		ShooterGameEnemyController.Instance.enemiesInWave--;
		ShooterGameEnemyController.Instance.CheckEnemiesInWave();
		ShooterGameManager.OnStateChanged -= OnGameStateChanged;
		Destroy(this.gameObject);
	}
}
