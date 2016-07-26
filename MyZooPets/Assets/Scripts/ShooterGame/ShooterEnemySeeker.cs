using UnityEngine;
using System.Collections;

public class ShooterEnemySeeker : ShooterEnemy {

	// Use this for initialization
	void Start () {
		StartCoroutine("FindingTarget");
	}

	IEnumerator FindingTarget() {
		yield return new WaitForSeconds(2.0f);
		LeanTween.move(this.gameObject, player.transform.position, moveDuration);
	}
}
