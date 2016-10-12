using UnityEngine;
using System.Collections;

public class ShooterEnemyBasic : ShooterEnemy{
	// basic ai just handles moving to the left and assigning values
	void Start(){
		animator.SetBool("IsSpitMode", false);
		LeanTween.moveX(this.gameObject, player.transform.position.x + -5, moveDuration).setOnComplete(OnOffScreen);
	}
}
