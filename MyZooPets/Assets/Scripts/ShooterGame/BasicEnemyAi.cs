using UnityEngine;
using System.Collections;

public class BasicEnemyAi : MonoBehaviour {

	public float Speed = 2.0f;
	public int ScoreVal=1;
	public int Damage = 1;
	private GameObject EnemyC;
	private GameObject Player;
	private bool paused;
	// Use this for initialization
	void Start () {
		EnemyC=GameObject.Find("ShooterGameManager");
		Player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged+= OnGameStateChanged;
	}
	
	// Update is called once per frame
	void Update () {
		if(ShooterGameManager.Instance.GetGameState()!= MinigameStates.Paused){
		transform.Translate (-Speed*Time.deltaTime,0,0);
		}
	}
	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			StartCoroutine("DestroyEnemy");
			break;
		case MinigameStates.Paused:
			paused = true;
			break;
		case MinigameStates.Playing:
			paused = false;
			break;
		}
	}
	void OnTriggerEnter2D(Collider2D Col){
		if(Col.gameObject.tag=="bullet")
		{
			EnemyC.GetComponent<ShooterGameManager>().AddScore(ScoreVal);
			Destroy(Col.gameObject);
			StartCoroutine("DestroyEnemy");
		}
		else if (Col.gameObject.tag=="Player"){
			Player.GetComponent<PlayerShooterController>().removeHealth(-Damage);
			StartCoroutine("DestroyEnemy");

		}
	}

	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		EnemyC.GetComponent<EnemyController>().EnemiesInWave--;
		EnemyC.GetComponent<EnemyController>().CheckEnemiesInWave();
		ShooterGameManager.OnStateChanged-= OnGameStateChanged;
		Destroy(this.gameObject);
	}
}
