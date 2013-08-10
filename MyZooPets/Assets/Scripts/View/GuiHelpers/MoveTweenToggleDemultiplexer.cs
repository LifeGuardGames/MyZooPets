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
	
	public bool isShowFinishedCallback = false;	// Call callback after the end of the tween instead of the beginning
	public GameObject ShowTarget;
	public string ShowFunctionName;
	public bool ShowIncludeChildren = false;
	
	public bool isHideFinishedCallback = false;	// Call callback after the end of the tween instead of the beginning
	public GameObject HideTarget;
	public string HideFunctionName;
	public bool HideIncludeChildren = false;
	
	private bool isShown;	// Active lock
	private bool isMoving;	// Move lock

	void Awake(){
		if(startsHidden){
			isShown = false;
			isMoving = false;
		}else{
			isShown = true;
			isMoving = false;
		}

		foreach(GameObject go in GoList){
			MoveTweenToggle toggle = go.GetComponent<MoveTweenToggle>();
			if(D.Assert(toggle != null, "No MoveTweenToggle script for " + go.GetFullName())){
				if(startsHidden){
					toggle.startsHidden = true;
					toggle.Reset();
				}
				else{
					toggle.startsHidden = false;
				}

				toggle.isDebug = false;	// Turn all the other debug off
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
				
				// If option set for finish show callback, call it now!
				if(isShowFinishedCallback){
					ShowSendCallback();
				}
				return;
			}
			if(!isShown && !lastFinishedHideObjectScript.IsMoving){
				isMoving = false;
				
				// If option set for finish hide callback, call it now!
				if(isHideFinishedCallback){
					HideSendCallback();
				}
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
				if(D.Assert(toggle != null, "No MoveTweenToggle script for " + go.GetFullName()))
					toggle.Show();
			}
			
			// If set to begin show callback, call it now!
			if(!isShowFinishedCallback){
				ShowSendCallback();
			}
		}
	}

	public void Hide(){
		if(isShown && !isMoving){
			isShown = false;
			isMoving = true;
			foreach(GameObject go in GoList){
				MoveTweenToggle toggle = go.GetComponent<MoveTweenToggle>();
				if(D.Assert(toggle != null, "No MoveTweenToggle script for " + go.GetFullName()))
					toggle.Hide();
			}
			
			// If set to begin hide callback, call it now!
			if(!isHideFinishedCallback){
				HideSendCallback();
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
	
	void ShowSendCallback(){
		if (string.IsNullOrEmpty(ShowFunctionName)) return;
		if (ShowTarget == null) ShowTarget = gameObject;

		if (ShowIncludeChildren){
			Transform[] transforms = ShowTarget.GetComponentsInChildren<Transform>();

			for (int i = 0, imax = transforms.Length; i < imax; ++i){
				Transform t = transforms[i];
				t.gameObject.SendMessage(ShowFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else{
			ShowTarget.SendMessage(ShowFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void HideSendCallback(){
		if (string.IsNullOrEmpty(HideFunctionName)) return;
		if (HideTarget == null) HideTarget = gameObject;
		
		if (HideIncludeChildren){
			Transform[] transforms = HideTarget.GetComponentsInChildren<Transform>();

			for (int i = 0, imax = transforms.Length; i < imax; ++i){
				Transform t = transforms[i];
				t.gameObject.SendMessage(HideFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else{
			HideTarget.SendMessage(HideFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
