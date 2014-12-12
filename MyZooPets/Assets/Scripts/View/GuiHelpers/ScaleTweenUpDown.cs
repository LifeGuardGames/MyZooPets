using UnityEngine;
using System.Collections;

/// <summary>
/// Scale tween up down.
/// Tweens scale for the duration and then reverses
/// </summary>

public class ScaleTweenUpDown : MonoBehaviour {
	
	public Vector3 scaleInit;
	public Vector3 scaleFactor;
	public float delay = 0;
	public float duration = 2f;
	
	private Vector3 scaleTo;
	
	
	// Use this for initialization
	void Start () {
		scaleInit = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		scaleTo = new Vector3(transform.localScale.x * scaleFactor.x, transform.localScale.y * scaleFactor.y, transform.localScale.z * scaleFactor.z);
		Invoke("StartScale", delay);
	}
	
	void StartScale(){
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "StartReverse");
		LeanTween.scale(gameObject, scaleTo, duration/2, optional);
	}
			
	void StartReverse(){
		Hashtable optional = new Hashtable();
		LeanTween.scale(gameObject, scaleInit, duration/2, optional);
	}
}
