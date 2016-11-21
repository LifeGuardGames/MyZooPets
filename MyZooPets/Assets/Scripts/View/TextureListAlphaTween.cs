using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tween sets of image's alphas, optional parameter for particles
/// </summary>
public class TextureListAlphaTween : MonoBehaviour {
	public List<GameObject> gameObjectList;
	public float tweenTime = 4;
	public float showAlpha = 1;
	public float hideAlpha = 0;
	public ParticleSystem optionalParticle;
	public bool isStartHidden = false;
	private List<int> currentTweenIdList;

	void Start(){
		if(isStartHidden){
			InstantHide();
		}
	}

	public void InstantHide(){
		ToggleParticle(false);
		foreach(GameObject go in gameObjectList){
			LeanTween.cancel(go);
			go.GetComponent<Renderer>().material.color = new Color(go.GetComponent<Renderer>().material.color.r,
			                                       go.GetComponent<Renderer>().material.color.g,
			                                       go.GetComponent<Renderer>().material.color.b,
			                                       hideAlpha);
		}
	}

	public void InstantShow(){
		ToggleParticle(true);
		foreach(GameObject go in gameObjectList){
			LeanTween.cancel(go);
			go.GetComponent<Renderer>().material.color = new Color(go.GetComponent<Renderer>().material.color.r,
			                                       go.GetComponent<Renderer>().material.color.g,
			                                       go.GetComponent<Renderer>().material.color.b,
			                                       showAlpha);
		}
	}

	public void Show(){
		ToggleParticle(true);
		foreach(GameObject go in gameObjectList){
			LeanTween.alpha(go, showAlpha, tweenTime);
		}
	}

	public void Hide(){
		ToggleParticle(false);
		foreach(GameObject go in gameObjectList){
			LeanTween.alpha(go, hideAlpha, tweenTime);
		}
	}

	public void ToggleParticle(bool isOn) {
		if(optionalParticle != null) {
			if(isOn) {
				optionalParticle.Play();
			}
			else {
				optionalParticle.Stop();
			}
		}
	}
}
