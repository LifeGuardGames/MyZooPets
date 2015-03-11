using UnityEngine;
using System.Collections;

public class ShooterEnemyHard:ShooterEnemy{
	private GameObject upper;
	private GameObject lower;
	private GameObject midPoint;
	public GameObject bulletPrefab;

	// Use this for initialization
	void Start(){
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.red;
		// movement positions
		upper = GameObject.Find("Upper");
		lower = GameObject.Find("Lower");
		midPoint = GameObject.Find("MidPoint");
		LeanTween.move(this.gameObject, midPoint.transform.position, speed).setOnComplete(MoveAgain);
	}

	// function that moves around the smog monster between two points randomly
	void MoveAgain(){
		if(Random.Range(0, 2) == 0){
			LeanTween.move(this.gameObject, upper.transform.position, speed).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject, lower.transform.position, speed).setOnComplete(ShootSmogBall);
		}
	}

	void AndAgain(){
		if(transform.position == upper.transform.position){
			LeanTween.move(this.gameObject, lower.transform.position, speed).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject, upper.transform.position, speed).setOnComplete(ShootSmogBall);
		}
	}

	// shoots a smog ball at the player
	void ShootSmogBall(){
		GameObject instance = Instantiate(bulletPrefab, this.gameObject.transform.position, bulletPrefab.transform.rotation)as GameObject;
		LeanTween.move(instance.gameObject, player.transform.position, 2.0f);
		StartCoroutine(HoldaSec());
	}

	// gives a 2 sec breather between shots
	IEnumerator HoldaSec(){
		yield return new WaitForSeconds(1.0f);
		AndAgain();
	}
}
