using UnityEngine;
using System.Collections;

public class ShooterHardEnemyAi :EnemyAiHandeler {


	private GameObject Player;
	private GameObject SkyPos;
	private GameObject Bottom;
	private GameObject MidPoint;
	private bool paused;
	public GameObject BulletPrefab;

	// Use this for initialization
	void Start () {
		Speed = 2.0f;
		ScoreVal = 5;
		health = 3;
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.gray;
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

	void ShootSmogBall(){
		GameObject instance = Instantiate(BulletPrefab,this.gameObject.transform.position,BulletPrefab.transform.rotation)as GameObject;
		LeanTween.move (instance,Player.transform.position,2.0f);
	}
}
