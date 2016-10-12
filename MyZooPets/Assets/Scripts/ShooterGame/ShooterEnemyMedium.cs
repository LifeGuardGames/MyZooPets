using UnityEngine;
using System.Collections;

public class ShooterEnemyMedium : ShooterEnemy{
	private GameObject skyPos;
	private GameObject bottom;
	
	// Use this for initialization
	void Start(){
		skyPos = GameObject.Find("Upper");
		bottom = GameObject.Find("Lower");
		if(Random.Range(0, 2) == 0){
			LeanTween.move(this.gameObject, skyPos.transform.position, moveDuration).setOnComplete(MoveAgain);
		}
		else{
			LeanTween.move(this.gameObject, bottom.transform.position, moveDuration).setOnComplete(MoveAgain);
		}
	}

	// moves once movement is complete makes a zigzag
	void MoveAgain(){
		LeanTween.move(this.gameObject, player.transform.position, moveDuration);
	}
}
