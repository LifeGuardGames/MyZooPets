using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle demultiplexer.
/// Packs multiple objects that needs to be move tweened into one compact call
/// </summary>

public class TweenToggleDemux : MonoBehaviour {

	public GameObject[] GoList;
	public Vector2 testButtonPos;
	public bool isDebug = false;
	public bool startsHidden = false;

	public GameObject lastFinishedShowObject;	// For lock
	private TweenToggle lastFinishedShowObjectScript;
	public GameObject lastFinishedHideObject;	// For lock
	private TweenToggle lastFinishedHideObjectScript;
	
	public bool isShowFinishedCallback = true;	// Call callback after the end of the tween instead of the beginning
	public GameObject ShowTarget;
	public string ShowFunctionName;
	public bool ShowIncludeChildren = false;
	
	public bool isHideFinishedCallback = true;	// Call callback after the end of the tween instead of the beginning
	public GameObject HideTarget;
	public string HideFunctionName;
	public bool HideIncludeChildren = false;
	
	public bool hideImmediately = false;
	
	private bool isShown; // Active lock
	private bool isMoving; // Move lock

	void Awake(){
		if(startsHidden){
			isShown = false;
			isMoving = false;
		}else{
			isShown = true;
			isMoving = false;
		}

		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			if(D.Assert(toggle != null, "No TweenToggle script for " + go.GetFullName())){
				if(startsHidden){
					toggle.startsHidden = true;	// TweenToggle Start() will take care of setting position
				}
				else{
					toggle.startsHidden = false;
				}

				toggle.isDebug = false;	// Turn all the other debug off
			}
		}
		
		if(lastFinishedShowObject != null){
			lastFinishedShowObjectScript = lastFinishedShowObject.GetComponent<TweenToggle>();
		}
		
		if(lastFinishedHideObject != null){
			lastFinishedHideObjectScript = lastFinishedHideObject.GetComponent<TweenToggle>();
		}
	}
	
	public void Reset(){
		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			toggle.startsHidden = startsHidden;
			toggle.Reset();
		}
	}
	
	void Update(){
		// Polling for lock released
		if(isMoving){
			print("demux moving");
			if(isShown && lastFinishedShowObjectScript != null && !lastFinishedShowObjectScript.IsMoving){
				print (isShown + " " + lastFinishedShowObjectScript + " " + lastFinishedShowObjectScript.IsMoving);
				isMoving = false;
				// If option set for finish show callback, call it now!
				if(isShowFinishedCallback){
				print ("SHOW CALLBACK");
					
					ShowSendCallback();
				}
				return;
			}
			if(!isShown && lastFinishedHideObjectScript != null && !lastFinishedHideObjectScript.IsMoving){
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
		StartCoroutine(SetNextFrameShow());
	}
	
	public void Hide(){
		StartCoroutine(SetNextFrameHide());
	}
	
	IEnumerator SetNextFrameShow(){
		yield return 0;
		if(!isShown && !isMoving){
			isShown = true;
			isMoving = true;
			
			foreach(GameObject go in GoList){
				TweenToggle toggle = go.GetComponent<TweenToggle>();
				if(D.Assert(toggle != null, "No TweenToggle script for " + go.GetFullName())){
					toggle.Show();
				}
			}
			
			// If set to begin show callback, call it now!
			if(!isShowFinishedCallback){
				ShowSendCallback();
			}
		}
	}
	
	IEnumerator SetNextFrameHide(){
		yield return 0;
		if(isShown && !isMoving){
			isShown = false;
			isMoving = true;
			foreach(GameObject go in GoList){
				TweenToggle toggle = go.GetComponent<TweenToggle>();
				if(D.Assert(toggle != null, "No TweenToggle script for " + go.GetFullName())){
					if(hideImmediately){
						Debug.Log(" -- - - HIDE BOOLEAN TRUE");
						// TODO Need to call last hide object last!!!!
						toggle.hideDuration = 0f;
						toggle.hideDelay = 0f;
					}
					toggle.Hide();
				}
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
