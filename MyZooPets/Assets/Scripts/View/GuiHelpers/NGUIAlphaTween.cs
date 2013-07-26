using UnityEngine;
using System.Collections;

public class NGUIAlphaTween : MonoBehaviour {

	private float currentAlpha;
	public float startAlpha = 1.0f;	// range [0-1]
	public float endAlpha = 0.0f;
	public float delay = 0;
	public float duration = 2f;

	private UIWidget widget;

	// Use this for initialization
	void Start () {
		widget = gameObject.GetComponent<UISprite>();

		if(startAlpha > 1f){
			startAlpha = 1f;
		}
		else if(startAlpha < 0){
			startAlpha = 0f;
		}
		widget.alpha = startAlpha;

		if(endAlpha > 1f){
			endAlpha = 1f;
		}
		else if(endAlpha < 0){
			endAlpha = 0f;
		}

		currentAlpha = startAlpha;

		StartAlphaTween();
	}

	void FixedUpdate(){
		widget.alpha = currentAlpha;
	}

	// LeanTween to update its own value
	public void StartAlphaTween(){
		Hashtable optional = new Hashtable();
		LeanTween.value(gameObject, "UpdateFloat", startAlpha, endAlpha, duration, optional);
	}

	public void UpdateFloat(float val){
		currentAlpha = val;
	}
}
