using UnityEngine;
using System.Collections;

/// <summary>
/// Scale tween up down.
/// Tweens scale for the duration and then reverses
/// </summary>

public class ScaleTweenUpDown : MonoBehaviour {
	
	public Vector3 scaleDelta;
	public float delay = 0;
	public float duration = 2f;
	
	private Vector3 scaleTo;
	
	
	// Use this for initialization
	void Start () {
		scaleTo = new Vector3(transform.localScale.x * scaleDelta.x, transform.localScale.y * scaleDelta.y, transform.localScale.z * scaleDelta.z);
		Invoke("StartScale", delay);
	}
	
	void StartScale(){
		Hashtable optional = new Hashtable();
		optional.Add("OnCompleteTarget", gameObject);
		optional.Add("OnComplete", "StartReverse");
		//optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.scale(gameObject, scaleTo, duration);
	}
			
	void StartReverse(){
		
	}
}
