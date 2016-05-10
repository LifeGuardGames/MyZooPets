//// Copyright (c) 2016 LifeGuard Games Inc.
using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle demultiplexer.
/// Packs multiple objects that needs to be move tweened into one compact call
/// </summary>
public class TweenToggleDemux : MonoBehaviour{

	public GameObject[] GoList;
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
	public bool IsShowing{ get { return isShown; } }

	private bool isMoving; // Move lock
	public bool IsMoving{ get { return isMoving; } }
	
	private bool pollingFirstFrameLock = false;	// Since we are polling & setting show/hide next frame, need to stop current frame check for callbacks
	private bool pollingSecondFrameLock = false;    // Check for two frames, dumb implementation to enforce order (update gets called before show)

	public CanvasGroup UIRayCastBlock;			// If this is assigned, it will attempt to block (ONLY) UI Raycasts when shown

	void Awake(){
		if(startsHidden){
			isShown = false;
			isMoving = false;
		}
		else{
			isShown = true;
			isMoving = false;
		}

		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			if(toggle != null){
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
	
	public void LgReset(){
		if(UIRayCastBlock != null) {
			UIRayCastBlock.blocksRaycasts = startsHidden ? false : true;
		}
		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			toggle.startsHidden = startsHidden;
			toggle.Reset();
		}
	}
	
	// Polling for lock released
	void Update(){
		// Stop polling check for the current frame tween starts!
		if(pollingFirstFrameLock){
			pollingFirstFrameLock = false;
		}
		else if(pollingSecondFrameLock){
			pollingSecondFrameLock = false;
		}
		// Do regular polling
		else{
			if(isMoving){
//				Debug.Log("is moving check" + lastFinishedShowObjectScript + " " + lastFinishedShowObjectScript.IsMoving);
				if(isShown && lastFinishedShowObjectScript != null && !lastFinishedShowObjectScript.IsMoving){
//					Debug.Log(isShown + " " + lastFinishedShowObjectScript + " " + lastFinishedShowObjectScript.IsMoving);
					isMoving = false;
					// If option set for finish show callback, call it now!
					if(isShowFinishedCallback){
						ShowSendCallback();
					}
					return;
				}
				else if(!isShown && lastFinishedHideObjectScript != null && !lastFinishedHideObjectScript.IsMoving){
					//print (isShown + " " + lastFinishedShowObjectScript + " " + lastFinishedShowObjectScript.IsMoving);
					isMoving = false;
					
					// If option set for finish hide callback, call it now!
					if(isHideFinishedCallback){
						HideSendCallback();
					}
					return;
				}
			}
		}
	}
	
	public void Show(){
		if(!isShown && !isMoving){
			isShown = true;
			isMoving = true;

			pollingFirstFrameLock = true;
			pollingSecondFrameLock = true;

			if(UIRayCastBlock != null) {
				UIRayCastBlock.blocksRaycasts = true;
			}

			StartCoroutine(SetNextFrameShow());
		}
		else{
			//Debug.Log(isShown + " + " + isMoving);
			//Debug.Log("Demux in locked state already");
		}
	}
	
	public void Hide(){
		if(isShown && !isMoving){
			isShown = false;
			isMoving = true;
			
			pollingFirstFrameLock = true;
			pollingSecondFrameLock = true;

			if(UIRayCastBlock != null) {
				UIRayCastBlock.blocksRaycasts = false;
			}

			StartCoroutine(SetNextFrameHide());
		}
		else{
			//Debug.Log(isShown + " + " + isMoving);
			//Debug.Log("Demux in locked state already");
		}
	}
	
	IEnumerator SetNextFrameShow(){
		yield return 0;
		
		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			if(toggle != null){
				toggle.Show();
			}
		}
		
		// If set to begin show callback, call it now!
		if(!isShowFinishedCallback){
			ShowSendCallback();
		}
	}
	
	IEnumerator SetNextFrameHide(){
		yield return 0;
		
		foreach(GameObject go in GoList){
			TweenToggle toggle = go.GetComponent<TweenToggle>();
			if(toggle != null){
				if(hideImmediately){
					//Debug.Log(" -- - - HIDE BOOLEAN TRUE");
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

//	 void OnGUI(){
//	 	 if(isDebug){
//	 	 	if(GUI.Button(new Rect(testButtonPos.x, testButtonPos.y, 100, 100), "show")){
//	 	 		Show();
//	 	 	}
//	 	 	if(GUI.Button(new Rect(testButtonPos.x + 110, testButtonPos.y, 100, 100), "hide")){
//				Debug.Log("HIDE button");
//	 	 		Hide();
//	 	 	}
//	 	 }
//	 }
	
	void ShowSendCallback(){
		if(string.IsNullOrEmpty(ShowFunctionName)){
			return;
		}
		if(ShowTarget == null){
			ShowTarget = gameObject;
		}
		if(ShowIncludeChildren){
			Transform[] transforms = ShowTarget.GetComponentsInChildren<Transform>();
			for(int i = 0, imax = transforms.Length; i < imax; ++i){
				Transform t = transforms[i];
				t.gameObject.SendMessage(ShowFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else{
			ShowTarget.SendMessage(ShowFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void HideSendCallback(){
		if(string.IsNullOrEmpty(HideFunctionName)){
			return;
		}
		if(HideTarget == null){
			HideTarget = gameObject;
		}
		if(HideIncludeChildren){
			Transform[] transforms = HideTarget.GetComponentsInChildren<Transform>();
			for(int i = 0, imax = transforms.Length; i < imax; ++i){
				Transform t = transforms[i];
				t.gameObject.SendMessage(HideFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else{
			HideTarget.SendMessage(HideFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
