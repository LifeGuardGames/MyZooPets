using UnityEngine;
using System.Collections;

public class EnemyBulletCollisionManager : MonoBehaviour {

	public bool isTrap;

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag =="Wall"){
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
