using UnityEngine;
using System.Collections;

public class ShooterEnemySmogWall : MonoBehaviour {

	void Start(){
		StartCoroutine("DestroyWall");
	}

	IEnumerator DestroyWall(){
		yield return new WaitForSeconds(2.0f);
		Destroy (this.gameObject);
	}
}
