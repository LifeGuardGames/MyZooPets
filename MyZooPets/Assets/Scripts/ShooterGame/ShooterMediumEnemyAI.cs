using UnityEngine;
using System.Collections;

public class ShooterMediumEnemyAi : ShooterEnemyAi{
	private GameObject skyPos;
	private GameObject bottom;
	
	// Use this for initialization
	void Start () {
		speed = 2.0f;
		scoreVal = 3;
		health = 2;
		damage = 2;
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.cyan;
		skyPos= GameObject.Find ("Upper");
		bottom= GameObject.Find("Lower");
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,skyPos.transform.position,speed).setOnComplete(MoveAgain);
		}
		else{
			LeanTween.move(this.gameObject,bottom.transform.position,speed).setOnComplete(MoveAgain);
			}
		}
	// moves once movement is complete makes a zigzag
	void MoveAgain(){
		LeanTween.move(this.gameObject,player.transform.position,speed);
	}
}
