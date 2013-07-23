using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle demultiplexer.
/// Packs multiple objects that needs to be move tweened into one compact call
/// </summary>

public class MoveTweenToggleDemultiplexer : MonoBehaviour {

	public GameObject[] GoList;
	public Vector2 testButtonPos;
	public bool isDebug = false;
	public bool startsHidden = false;

	public GameObject lastFinishedShowObject;	// For lock
	private MoveTweenToggle lastFinishedShowObjectScript;
	public GameObject lastFinishedHideObject;	// For lock
	private MoveTweenToggle lastFinishedHideObjectScript;


	private bool isShown;
	private bool isMoving;

	void Awake(){
		//Debug.Log("Demux awake");
		if(startsHidden){
			isShown = false;
			isMoving = false;
		}else{
			isShown = true;
			isMoving = false;
		}

		foreach(GameObject go in GoList){
			MoveTweenToggle toggle = go.GetComponent<MoveTweenToggle>();
			if(toggle != null){
				if(startsHidden){
					toggle.startsHidden = true;
					toggle.Reset();
					// go.transform.localPosition = new Vector3(
					// 	go.transform.localPosition.x + toggle.hideDeltaX,
					// 	go.transform.localPosition.y + toggle.hideDeltaY,
					// 	go.transform.localPosition.z
					// );
				}
				else{
					toggle.startsHidden = false;
				}

				toggle.isDebug = false;	// Turn all the other debug off
			}
			else{
				Debug.LogError("No MoveTweenToggle script for " + go.GetFullName());
			}
		}

		lastFinishedShowObjectScript = lastFinishedShowObject.GetComponent<MoveTweenToggle>();
		lastFinishedHideObjectScript = lastFinishedHideObject.GetComponent<MoveTweenToggle>();
	}

	void Update(){
		// Polling for lock released
		if(isMoving){
			if(isShown && !lastFinishedShowObjectScript.IsMoving){
				isMoving = false;
				return;
			}
			if(!isShown && !lastFinishedHideObjectScript.IsMoving){
				isMoving = false;
				return;
			}
		}
	}

	public void Show(){
		if(!isShown && !isMoving){
			isShown = true;
			isMoving = true;
			foreach(GameObject go in GoList){
				MoveTweenToggle toggle = go.GetComponent<MoveTweenToggle>();
				if(toggle != null){
					toggle.Show();
				}
				else{
					Debug.LogError("No MoveTweenToggle script for " + go.GetFullName());
				}
			}
		}
	}

	public void Hide(){
		if(isShown && !isMoving){
			isShown = false;
			isMoving = true;
			foreach(GameObject go in GoList){
				MoveTweenToggle toggle = go.GetComponent<MoveTweenToggle>();
				if(toggle != null){
					toggle.Hide();
				}
				else{
					Debug.LogError("No MoveTweenToggle script for " + go.GetFullName());
				}
			}
		}
	}

	void OnGUI(){
		if(isDebug){
			if(GUI.Button(new Rect(testButtonPos.x, testButtonPos.y, 100, 100), "show")){
				Show();
			}
			if(GUI.Button(new Rect(testButtonPos.x + 110, testButtonPos.y, 100, 100), "hide")){
				Hide();
			}
		}
	}
}
