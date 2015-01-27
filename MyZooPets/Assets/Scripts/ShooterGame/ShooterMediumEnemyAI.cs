using UnityEngine;
using System.Collections;

public class ShooterMediumEnemyAi : ShooterEnemyAi{
	private GameObject SkyPos;
	private GameObject Bottom;
	
	// Use this for initialization
	void Start () {
		Speed = 2.0f;
		ScoreVal = 3;
		health = 2;
		Damage = 2;
		this.gameObject.GetComponentInChildren<UISprite>().color = Color.cyan;
		SkyPos= GameObject.Find ("Upper");
		Bottom= GameObject.Find("Lower");
		if (Random.Range (0,2)==0){
			LeanTween.move(this.gameObject,SkyPos.transform.position,Speed).setOnComplete(MoveAgain);
		}
		else{
			LeanTween.move(this.gameObject,Bottom.transform.position,Speed).setOnComplete(MoveAgain);
			}
		}

	void MoveAgain(){
		LeanTween.move(this.gameObject,Player.transform.position,Speed);
	}
}
