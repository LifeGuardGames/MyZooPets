using UnityEngine;
using System.Collections;

public class ShooterEnemyBasic : ShooterEnemy{
	// basic ai just handles moving to the left and assigning values
	void Start(){
		LeanTween.moveX(this.gameObject, player.transform.position.x, speed);
	}
}
