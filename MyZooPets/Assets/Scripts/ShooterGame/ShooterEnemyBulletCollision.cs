using UnityEngine;
using System.Collections;

public class ShooterEnemyBulletCollision : MonoBehaviour {

	public bool isTrap;

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag =="ShooterWall"){
			if(isTrap){
				StartCoroutine("TrapTimer");
			}
			else{
				Destroy(this.gameObject);
			}
		}
	}

	private IEnumerator TrapTimer(){
		yield return new WaitForSeconds(5.0f);
		Destroy(this.gameObject);
	}
}
