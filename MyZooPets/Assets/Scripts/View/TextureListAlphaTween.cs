﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureListAlphaTween : MonoBehaviour {

	public List<GameObject> gameObjectList;
	public float tweenTime = 4;
	public float showAlpha = 1;
	public float hideAlpha = 0;
	public bool isStartHidden = false;
	private List<int> currentTweenIdList;

	void Start(){
		if(isStartHidden){
			InstantHide();
		}
	}

	public void InstantHide(){
		foreach(GameObject go in gameObjectList){
			go.renderer.material.color = new Color(go.renderer.material.color.r,
			                                       go.renderer.material.color.g,
			                                       go.renderer.material.color.b,
			                                       hideAlpha);
			Debug.Log("HIDING ALPHA");
		}
	}

	public void InstantShow(){
		foreach(GameObject go in gameObjectList){
			Debug.Log("SHOWING ALPHA");
			go.renderer.material.color = new Color(go.renderer.material.color.r,
			                                       go.renderer.material.color.g,
			                                       go.renderer.material.color.b,
			                                       showAlpha);
		}
	}

	public void Show(){
		foreach(GameObject go in gameObjectList){
			LTDescr description = LeanTween.alpha(go, showAlpha, tweenTime);
		}
	}

	public void Hide(){
		foreach(GameObject go in gameObjectList){
			LTDescr description = LeanTween.alpha(go, hideAlpha, tweenTime);
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
//			StartTweeningForward();
//		}
//		else if(GUI.Button(new Rect(200, 100, 100, 100), "testbackwards")){
//			StartTweeningBackward();
//		}
//	}
}
