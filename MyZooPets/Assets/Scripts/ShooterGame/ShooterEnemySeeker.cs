using UnityEngine;
using System.Collections;

public class ShooterEnemySeeker : ShooterEnemy {

	// Use this for initialization
	void Start () {
		LeanTween.move(this.gameObject, player.transform.position, moveDuration);
	}
}
