using UnityEngine;
using System.Collections;

public class TutEnemy : MonoBehaviour {

	void Start(){
		this.gameObject.layer = 2;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "bullet"){
				Destroy(collider.gameObject);
				ShooterGameManager.Instance.MoveTut();
				StartCoroutine(DestroyEnemy());
		}
	}

	// this is a coroutine to make sure enemies are destroyed at the end of frame otherwise an error is thrown by NGUI
	IEnumerator DestroyEnemy(){
		yield return new WaitForEndOfFrame();
		GetComponent<Collider2D>().enabled = false;
		
		// Wait until the particles has finished clearing before you destroy
		Destroy(this.gameObject);
	}
}
