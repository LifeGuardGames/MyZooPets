using UnityEngine;
using System.Collections;

public class TweenRotateLocalLoop : MonoBehaviour {

	public Vector3 startRotation;
	public Vector3 endRotation;
	public float duration;
	public bool isPingPong = false;
	
	// Use this for initialization
	void Start(){
		if(duration <= 0){
			Debug.LogWarning("Duration can not be 0 or negative");
			duration = 1f;
		}
		gameObject.transform.localEulerAngles = startRotation;

		if(isPingPong){
			LeanTween.rotateLocal(this.gameObject, endRotation, duration)
				.setRepeat(-1)
					.setLoopPingPong()
					.setEase(LeanTweenType.easeInOutQuad);
		}
		// Use clamp
		else{
			LeanTween.rotateLocal(this.gameObject, endRotation, duration)
				.setRepeat(-1)
					.setLoopClamp()
					.setEase(LeanTweenType.easeInOutQuad);
		}
	}
}
