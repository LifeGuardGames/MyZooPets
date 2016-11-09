using UnityEngine;
using System.Collections;

public class MiniPetPowerUp : MonoBehaviour {
	public GameObject upPos;
	public GameObject downPos;
	public GameObject bulletPrefab;
	private GameObject bulletAux;

	void Start() {
		WakeUp();
	}

	public void WakeUp() {
		if(!this.gameObject.activeSelf) {
			this.gameObject.SetActive(true);
			StartCoroutine("Fire");
		}
	}

	IEnumerator Fire() {
		yield return new WaitForSeconds(0.5f);
		bulletAux = GameObjectUtils.AddChild(null, bulletPrefab);
		bulletAux.transform.position = transform.position;
		LeanTween.moveX(bulletAux, 100.0f, 4.0f);
		StartCoroutine("Move");
	}

	IEnumerator Move() {
		yield return new WaitForSeconds(1.0f);
		if(transform.position.y == upPos.transform.position.y) {
			LeanTween.moveY(this.gameObject, downPos.transform.position.y, 1.0f);
          }
		else {
			LeanTween.moveY(this.gameObject, upPos.transform.position.y, 1.0f);
		}
		StartCoroutine("Fire");
	}

	public void Hit() {
		StopAllCoroutines();
		this.gameObject.SetActive(false);
	}

	void OnTriggerEnter2D (Collider2D col) {
		if(col.gameObject.tag == "ShooterEnemy" || col.gameObject.tag == "ShooterEnemyBullet") {
			Hit();
		}
	}
}
