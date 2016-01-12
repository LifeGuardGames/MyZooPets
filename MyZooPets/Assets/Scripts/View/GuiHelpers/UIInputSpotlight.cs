using UnityEngine;
using System.Collections;

// Bad hack to get the menuscene spotlighting going os user doesnt click other stuff
public class UIInputSpotlight:MonoBehaviour{

	public GameObject spotlightToggle;
	public UIInput inputScript;

	private bool isActive;

	void Start(){
		spotlightToggle.SetActive(false);
		isActive = false;
	}

	// Polling - inefficient
	void Update(){
		if(inputScript.selected != isActive){
			isActive = inputScript.selected;

			spotlightToggle.SetActive(isActive ? true : false);
		}
	}
}
