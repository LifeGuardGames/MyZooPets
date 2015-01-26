using UnityEngine;
using System.Collections;

public class EnemyAiHandeler : MonoBehaviour {

	public float Speed;
	public int ScoreVal;
	public int Damage;
	public int health;
	protected GameObject Player;

	// Use this for initialization
	void Awake () {
		Player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged+= OnGameStateChanged;
	}

	// Update is called once per frame
	void Update () {
		if(ShooterGameManager.Instance.GetGameState()== MinigameStates.GameOver){
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
		if(collider.gameObject.tag=="bullet")
		{
			
			Destroy(collider.gameObject);
			health--;
			if (health <= 0){
				ShooterGameManager.Instance.AddScore(ScoreVal);
				StartCoroutine(DestroyEnemy());
			}
		}
		else if (collider.gameObject.tag=="Player"){
			PlayerShooterController.Instance.removeHealth(-Damage);
			StartCoroutine(DestroyEnemy());
		}
	}
	
	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		LeanTween.cancel(this.gameObject);
		ShooterGameEnemyController.Instance.EnemiesInWave--;
		ShooterGameEnemyController.Instance.CheckEnemiesInWave();
		ShooterGameManager.OnStateChanged-= OnGameStateChanged;
		Destroy(this.gameObject);
	}
}
