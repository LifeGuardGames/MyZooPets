using UnityEngine;
using System.Collections;

public class TweenMoveLocalLoop : MonoBehaviour{

	public Vector3 startPosition;
	public Vector3 endPosition;
	public float duration;
	public LeanTweenType easeType = LeanTweenType.linear;
	public bool isPingPong;

	// Use this for initialization
	void Start(){
		if(duration <= 0){
			Debug.LogWarning("Duration can not be 0 or negative");
			duration = 1f;
		}
		gameObject.transform.localPosition = startPosition;

		if(isPingPong){
			LeanTween.moveLocal(this.gameObject, endPosition, duration).setRepeat(-1).setLoopPingPong().setEase(easeType);
		}
		else{
			LeanTween.moveLocal(this.gameObject, endPosition, duration).setRepeat(-1).setLoopClamp().setEase(easeType);
		}
	}
}
