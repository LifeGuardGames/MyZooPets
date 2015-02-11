using UnityEngine;
using System.Collections;

public class ShooterBasicEnemyAi:ShooterEnemyAi{
	// basic ai just handles moveing to the left and assigning values
	void Start () {
		speed = 1.5f;
		scoreVal = 1;
		damage = 1;
		health = 1;
		LeanTween.moveX(this.gameObject,player.transform.position.x,speed);
	}
}
