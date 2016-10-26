using UnityEngine;
using System.Collections;

public class ShooterEnemySeeker : ShooterEnemy {
	private bool top = false;
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
		top = true;
		LeanTween.move(this.gameObject, ShooterSpawnManager.Instance.SeekerTopPosition, 1.0f);
		StartCoroutine("Seeking");
	}

	IEnumerator Seeking() {
		yield return new WaitForSeconds(3.0f);
		if(top) {
			LeanTween.move(this.gameObject, player.transform.position + new Vector3(-5, -5, 0), moveDuration).setOnComplete(OnOffScreen);
		}
		else {
			LeanTween.move(this.gameObject, player.transform.position + new Vector3(-5, 5, 0), moveDuration).setOnComplete(OnOffScreen);
		}
	}

}
