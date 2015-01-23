using UnityEngine;
using System.Collections;

public class ShooterMediumEnemyAI : MonoBehaviour {

	public float Speed = 2.0f;
	public int ScoreVal = 3;
	public int Damage = 2;
	public int health = 2;
	private GameObject EnemyC;
	private GameObject Player;
	GameObject SkyPos;
	GameObject Bottom;
	private bool paused;
	// Use this for initialization
	void Start () {
		EnemyC=GameObject.Find("ShooterGameManager");
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.cyan;
		Player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged+= OnGameStateChanged;
		SkyPos= GameObject.Find ("Upper");
		Bottom= GameObject.Find("Lower");
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,SkyPos.transform.position,Speed).setOnComplete(MoveAgain);
		}
		else{
			LeanTween.move(this.gameObject,Bottom.transform.position,Speed).setOnComplete(MoveAgain);
			}
		}

	void MoveAgain(){
		LeanTween.move(this.gameObject,Player.transform.position,Speed);
	}
	// Update is called once per frame
	void Update () {
		if(ShooterGameManager.Instance.GetGameState()== MinigameStates.GameOver){
			StartCoroutine("DestroyEnemy");
		}
	}
	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			StartCoroutine("DestroyEnemy");
			break;
		case MinigameStates.Paused:
			LeanTween.pause(this.gameObject);
			break;
		case MinigameStates.Playing:
			LeanTween.resume(this.gameObject);
			break;
		case MinigameStates.Restarting:
			StartCoroutine("DestroyEnemy");
			break;
		}
	}
	void OnTriggerEnter2D(Collider2D Col){
		if(Col.gameObject.tag=="bullet")
		{

			Destroy(Col.gameObject);
			health--;
			if (health <= 0){
				EnemyC.GetComponent<ShooterGameManager>().AddScore(ScoreVal);
				StartCoroutine("DestroyEnemy");
			}
		}
		else if (Col.gameObject.tag=="Player"){
			Player.GetComponent<PlayerShooterController>().removeHealth(-Damage);
			StartCoroutine("DestroyEnemy");
			
		}
	}
	
	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		LeanTween.cancel(this.gameObject);
		EnemyC.GetComponent<ShooterGameEnemyController>().EnemiesInWave--;
		EnemyC.GetComponent<ShooterGameEnemyController>().CheckEnemiesInWave();
		ShooterGameManager.OnStateChanged-= OnGameStateChanged;
		Destroy(this.gameObject);
	}
}
