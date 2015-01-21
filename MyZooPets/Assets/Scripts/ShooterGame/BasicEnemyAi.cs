using UnityEngine;
using System.Collections;

public class BasicEnemyAi : MonoBehaviour {

	public float Speed = 2.0f;
	public int ScoreVal=1;
	public int Damage = 1;
	private GameObject EnemyC;
	private GameObject Player;
	// Use this for initialization
	void Start () {
		EnemyC=GameObject.Find("ShooterGameManager");
		Player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-Speed*Time.deltaTime,0,0);
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
		Destroy(this.gameObject);
	}
}
