using UnityEngine;
using System.Collections;

// Bad hack to get the menuscene spotlighting going os user doesnt click other stuff
public class UIInputSpotlight:MonoBehaviour{

	public GameObject spotlightToggle;
	public UIInput inputScript;

	private bool enabled;

	void Start(){
		spotlightToggle.SetActive(false);
		enabled = false;
	}

	// Polling - inefficient
	void Update(){
		if(inputScript.selected != enabled){
			enabled = inputScript.selected;

			spotlightToggle.SetActive(enabled ? true : false);
		}
	}
}
