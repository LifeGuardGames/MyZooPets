using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGUIAlphaTween : MonoBehaviour {

	public float startAlpha = 1.0f;	// range [0-1]
	public float endAlpha = 0.0f;
	public float delay = 0;
	public float duration = 2f;

	public GameObject onCompleteTarget;
	public string onCompleteFunctionName;

	public bool isTweeningPanel = false;	// Check this when you are tweening a panel instead of a widget
	private UIPanel panel;
	private UIWidget widget;
	private bool isTweening = false;

	void Awake () {
		panel = GetComponent<UIPanel>();
		widget = GetComponent<UIWidget>();
	}

	void Start(){
		Initialize();
	}

	private void Initialize(){
		if(startAlpha > 1f){
			startAlpha = 1f;
		}
		else if(startAlpha < 0){
			startAlpha = 0f;
		}
		
		if(isTweeningPanel){
			panel.alpha = startAlpha;
		}
		else{
			widget.alpha = startAlpha;
		}
		
		if(endAlpha > 1f){
			endAlpha = 1f;
		}
		else if(endAlpha < 0){
			endAlpha = 0f;
		}
	}

	public void StartAlphaTween(float _startAlpha, float _endAlpha, float _delay, float _duration){
		startAlpha = _startAlpha;
		endAlpha = _endAlpha;
		delay = _delay;
		duration = _duration;

		Initialize();
		StartAlphaTween();
	}

	// LeanTween to update its own value
	public void StartAlphaTween(){
		if(!isTweening){
			isTweening = true;
			LeanTween.value(gameObject, UpdateFloat, startAlpha, endAlpha, duration)
				.setOnComplete(OnCompleteCallback);
		}
		else{
			Debug.LogError("alpha tween already tweening");
		}
	}

	public void UpdateFloat(float val){
		if(isTweeningPanel){
			panel.alpha = val;
		}
		else{
			widget.alpha = val;
		}
	}

	private void OnCompleteCallback(){
		isTweening = false;
		if(string.IsNullOrEmpty(onCompleteFunctionName)){
			return;
		}
		if(onCompleteTarget == null){
			onCompleteTarget = gameObject;
		}
		else{
			onCompleteTarget.SendMessage(onCompleteFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
