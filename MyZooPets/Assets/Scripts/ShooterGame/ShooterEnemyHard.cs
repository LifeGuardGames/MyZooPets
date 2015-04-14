using UnityEngine;
using System.Collections;

public class ShooterEnemyHard:ShooterEnemy{
	private GameObject upper;
	private GameObject lower;
	private GameObject midPoint;
	public GameObject bulletPrefab;

	// Use this for initialization
	void Start(){
		animator.SetBool("IsSpitMode", true);
		// movement positions
		upper = GameObject.Find("Upper");
		lower = GameObject.Find("Lower");
		midPoint = GameObject.Find("MidPoint");
		LeanTween.move(this.gameObject, midPoint.transform.position, moveDuration).setOnComplete(MoveAgain);
	}

	// function that moves around the smog monster between two points randomly
	void MoveAgain(){
		if(Random.Range(0, 2) == 0){
			LeanTween.move(this.gameObject, upper.transform.position, moveDuration).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject, lower.transform.position, moveDuration).setOnComplete(ShootSmogBall);
		}
	}

	void AndAgain(){
		if(transform.position == upper.transform.position){
			LeanTween.move(this.gameObject, lower.transform.position, moveDuration).setOnComplete(ShootSmogBall);
		}
		else{
			LeanTween.move(this.gameObject, upper.transform.position, moveDuration).setOnComplete(ShootSmogBall);
		}
	}

	// shoots a smog ball at the player
	void ShootSmogBall(){
		animator.SetBool("Spit",true);
		animator.SetBool("IsSpitMode", false);
		if(!isDead){
			GameObject instance = Instantiate(bulletPrefab, this.gameObject.transform.position, bulletPrefab.transform.rotation)as GameObject;
			LeanTween.move(instance.gameObject, player.transform.position, 2.0f);
			StartCoroutine(WaitASecond());
		}
	}

	// gives a 2 sec breather between shots
	IEnumerator WaitASecond(){
		yield return new WaitForSeconds(1.0f);
		AndAgain();
	}

	void OnDestroy(){
		LeanTween.cancel(this.gameObject);

		Debug.Log("sucess");
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "sdfsf")){
//			ShootSmogBall();
//		}
//	}
}
