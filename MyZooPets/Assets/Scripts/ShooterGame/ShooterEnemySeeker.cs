using UnityEngine;
using System.Collections;

public class ShooterEnemySeeker : ShooterEnemy {

	public void InitTop() {
		StartCoroutine("FindingTargetTop");
	}
	public void InitBottom() {
		StartCoroutine("FindingTargetBottom");
	}

	IEnumerator FindingTargetBottom() {
		yield return new WaitForSeconds(2.0f);
		LeanTween.move(this.gameObject,ShooterSpawnManager.Instance.SeekerBottomPosition, 1.0f);
		StartCoroutine("Seeking");
	}

	IEnumerator FindingTargetTop() {
		yield return new WaitForSeconds(2.0f);
		LeanTween.move(this.gameObject, ShooterSpawnManager.Instance.SeekerTopPosition, 1.0f);
		StartCoroutine("Seeking");
	}

	IEnumerator Seeking() {
		yield return new WaitForSeconds(3.0f);
		LeanTween.move(this.gameObject, player.transform.position + new Vector3(-5, 0, 0), moveDuration).setOnComplete(OnOffScreen);
    }

}
