using UnityEngine;
using System.Collections;

public class MiniPetFireBall : MonoBehaviour {

	void OnBecameInvisible() {
		Destroy(this.gameObject);
	}
}
