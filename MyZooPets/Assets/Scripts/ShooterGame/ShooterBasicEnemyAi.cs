using UnityEngine;
using System.Collections;

public class ShooterBasicEnemyAi:ShooterEnemyAi{
	// Use this for initialization
	void Start () {
		speed = 2.5f;
		scoreVal=1;
		damage = 1;
		health=1;
		LeanTween.moveX(this.gameObject,player.transform.position.x,speed);
	}
}
