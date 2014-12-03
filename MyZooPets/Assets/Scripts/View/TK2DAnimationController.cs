using UnityEngine;
using System.Collections;

public class TK2DAnimationController : MonoBehaviour {

	public GameObject animatorObject;
	protected tk2dSpriteAnimator animator;

	public bool isPlayAutomatically = false;

	void Start () {
		Initialize();
	}

	protected void Initialize(){
		animator = animatorObject.GetComponent<tk2dSpriteAnimator>();
		if(animator == null){
			Debug.LogError("No animator found");
		}

		if(isPlayAutomatically){
			animator.playAutomatically = true;
		}
	}
}
