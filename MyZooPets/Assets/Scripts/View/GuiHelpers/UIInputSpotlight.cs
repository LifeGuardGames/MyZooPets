using UnityEngine;
using System.Collections;

// Bad hack to get the menuscene spotlighting going os user doesnt click other stuff
public class UIInputSpotlight:MonoBehaviour{

	public GameObject spotlightToggle;
	public UIInput inputScript;

	private bool isEnabled;

	void Start(){
		spotlightToggle.SetActive(false);
		isEnabled = false;
	}

	// Polling - inefficient
	void Update(){
		if(inputScript.selected != isEnabled){
			isEnabled = inputScript.selected;

			spotlightToggle.SetActive(isEnabled ? true : false);
		}
	}
}
