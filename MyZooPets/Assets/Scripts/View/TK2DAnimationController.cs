using UnityEngine;
using System.Collections;

public class TK2DAnimationController : MonoBehaviour {

	public GameObject animatorObject;
	protected tk2dSpriteAnimator animator;

	public bool isPlayAutomatically = false;

	void Start () {
		animator = animatorObject.GetComponent<tk2dSpriteAnimator>();
		D.Assert(animator != null, "No animator found");

		if(isPlayAutomatically){
			animator.playAutomatically = true;
		}
	}
}
