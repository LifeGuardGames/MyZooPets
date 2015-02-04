using UnityEngine;
using System.Collections;

public class ShooterHardEnemyAi:ShooterEnemyAi {

	private GameObject skyPos;
	private GameObject Bottom;
	private GameObject MidPoint;
	public GameObject bulletPrefab;

	// Use this for initialization
	void Start () {
		bulletPrefab = this.gameObject.GetComponent<Enemy>().bulletPrefab;
		speed = 1.0f;
		scoreVal = 5;
		health = 3;
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.red;
		// movement positions
		skyPos = GameObject.Find("Upper");
		Bottom = GameObject.Find("Lower");
		MidPoint = GameObject.Find("MidPoint");
		LeanTween.move(this.gameObject,MidPoint.transform.position,speed).setOnComplete(MoveAgain);
	}
	// function that moves around the smog monster between two points randomly
	void MoveAgain(){
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,skyPos.transform.position,speed).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject,Bottom.transform.position,speed).setOnComplete(ShootSmogBall);
		}
	}
	// shoots a smog ball at the player
	void ShootSmogBall(){
		GameObject instance = Instantiate(bulletPrefab,this.gameObject.transform.position,bulletPrefab.transform.rotation)as GameObject;
		LeanTween.move (instance.gameObject,player.transform.position,2.0f);
		StartCoroutine(HoldaSec());
	}
	// gives a 2 sec breather between shots
	IEnumerator HoldaSec(){
		yield return new WaitForSeconds(2.0f);
		MoveAgain();
	}
}
