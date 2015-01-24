using UnityEngine;
using System.Collections;

public class ShooterEnemyBullet : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player"){
			
		}
	}
}
