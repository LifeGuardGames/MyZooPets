using UnityEngine;
using System.Collections;

public class ShooterHardEnemyAi :EnemyAiHandeler {

	private GameObject skyPos;
	private GameObject Bottom;
	private GameObject MidPoint;
	public GameObject BulletPrefab;

	// Use this for initialization
	void Start () {
		BulletPrefab = this.gameObject.GetComponent<Enemy>().bulletPrefab;
		Speed = 1.0f;
		ScoreVal = 5;
		health = 3;
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.red;
		skyPos = GameObject.Find ("Upper");
		Bottom = GameObject.Find("Lower");
		MidPoint = GameObject.Find("MidPoint");
		LeanTween.move(this.gameObject,MidPoint.transform.position,Speed).setOnComplete(MoveAgain);
	}
	
	void MoveAgain(){
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,skyPos.transform.position,Speed).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject,Bottom.transform.position,Speed).setOnComplete(ShootSmogBall);
		}
	}

	void ShootSmogBall(){
		GameObject instance = Instantiate(BulletPrefab,this.gameObject.transform.position,BulletPrefab.transform.rotation)as GameObject;
		LeanTween.move (instance.gameObject,Player.transform.position,2.0f);
		StartCoroutine(HoldaSec());
	}

	IEnumerator HoldaSec(){
		yield return new WaitForSeconds(2.0f);
		MoveAgain();
	}
}
