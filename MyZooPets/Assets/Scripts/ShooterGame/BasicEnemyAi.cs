using UnityEngine;
using System.Collections;

public class BasicEnemyAi : MonoBehaviour {

	public float Speed = 2.0f;
	private GameObject EnemyC;
	// Use this for initialization
	void Start () {
		EnemyC=GameObject.Find("ShooterGameManager");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-Speed*Time.deltaTime,0,0);
	}
	void OnTriggerEnter2D(Collider2D Col){
		if(Col.gameObject.tag=="bullet")
		{
			//Player.GetComponent<Player>().AddScore(ScoreVal);
			Destroy(Col.gameObject);
			StartCoroutine("DestroyEnemy");
		}
		else if (Col.gameObject.tag=="Player"){
			//Player.GetComponent<Player>().removeHealth(-Damage);
			StartCoroutine("DestroyEnemy");

		}
	}
	IEnumerator DestroyEnemy(){
		Debug.Log("hi");
		yield return new WaitForEndOfFrame();
		EnemyC.GetComponent<EnemyController>().EnemiesInWave--;
		Destroy(this.gameObject);
	}
}
