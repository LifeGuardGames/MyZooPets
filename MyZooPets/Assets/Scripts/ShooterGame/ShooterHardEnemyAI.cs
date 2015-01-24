using UnityEngine;
using System.Collections;

public class ShooterHardEnemyAI : MonoBehaviour {

	public float Speed = 2.0f;
	public int ScoreVal = 5;
	//public int Damage = 2;
	public int health = 3;
	private GameObject EnemyC;
	private GameObject Player;
	GameObject SkyPos;
	GameObject Bottom;
	GameObject MidPoint;
	private bool paused;
	public GameObject BulletPrefab;
	// Use this for initialization
	void Start () {
		EnemyC=GameObject.Find("ShooterGameManager");
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.cyan;
		Player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged+= OnGameStateChanged;
		SkyPos = GameObject.Find ("Upper");
		Bottom = GameObject.Find("Lower");
		MidPoint =  GameObject.Find("MidPoint");
		LeanTween.move(this.gameObject,MidPoint.transform.position,Speed).setOnComplete(MoveAgain);
	}
	
	void MoveAgain(){
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,SkyPos.transform.position,Speed).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject,Bottom.transform.position,Speed).setOnComplete(MoveAgain);
		}
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

	}
	void ShootSmogBall(){
		GameObject instance = Instantiate(BulletPrefab,this.gameObject.transform.position,BulletPrefab.transform.rotation)as GameObject;
		LeanTween.move (instance,Player.transform.position,2.0f);
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
