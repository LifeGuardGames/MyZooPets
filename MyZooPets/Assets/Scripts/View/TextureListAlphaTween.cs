using UnityEngine;
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
			LeanTween.cancel(go);
			go.GetComponent<Renderer>().material.color = new Color(go.GetComponent<Renderer>().material.color.r,
			                                       go.GetComponent<Renderer>().material.color.g,
			                                       go.GetComponent<Renderer>().material.color.b,
			                                       hideAlpha);
		}
	}

	public void InstantShow(){
		foreach(GameObject go in gameObjectList){
			LeanTween.cancel(go);
			go.GetComponent<Renderer>().material.color = new Color(go.GetComponent<Renderer>().material.color.r,
			                                       go.GetComponent<Renderer>().material.color.g,
			                                       go.GetComponent<Renderer>().material.color.b,
			                                       showAlpha);
		}
	}

	public void Show(){
		foreach(GameObject go in gameObjectList){
			LeanTween.alpha(go, showAlpha, tweenTime);
		}
	}

	public void Hide(){
		foreach(GameObject go in gameObjectList){
			LeanTween.alpha(go, hideAlpha, tweenTime);
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
