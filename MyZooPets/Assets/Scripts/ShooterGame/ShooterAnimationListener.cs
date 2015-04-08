using UnityEngine;
using System.Collections;

public class ShooterAnimationListener : MonoBehaviour {

	public void StopSpitting(){
		this.gameObject.GetComponent<Animator>().SetBool("Spit",false);
		this.gameObject.GetComponent<Animator>().SetBool("IsSpitMode", true);
	}
}
