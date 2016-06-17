using UnityEngine;
using System.Collections;

public class GlindaAnimationTest : MonoBehaviour {
	public Animator animator;

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "attackModeTrue")){
			animator.SetBool("IsAttacking", true);
		}
		if(GUI.Button(new Rect(200, 100, 100, 100), "attackModeFalse")){
			animator.SetBool("IsAttacking", false);
		}
	}
}
