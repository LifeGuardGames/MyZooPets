using UnityEngine;
using System.Collections;

public class ShooterEnemySeeker : ShooterEnemy {

	// Use this for initialization
	void Start () {
		StartCoroutine("FindingTarget");
	}

	IEnumerator FindingTarget() {
		yield return new WaitForSeconds(2.0f);
		LeanTween.move(this.gameObject, new Vector3 (player.transform.position.x- (player.transform.position.x - 100), player.transform.position.y-(player.transform.position.y - 100), player.transform.position.z), 1.0f);
		StartCoroutine("Seeking");
	}
	IEnumerator Seeking() {
		yield return new WaitForSeconds(3.0f);
		LeanTween.move(this.gameObject, player.transform.position, moveDuration);
    }

}
