//// Copyright (c) 2015 LifeGuard Games Inc.

using UnityEngine;
using System.Collections;

/// <summary>
/// Tween toggle.
/// Used to toogle move objects with LeanTween
/// Parent class not to be used, implemented by PositionTweenToggle, ScaleTweenToggle, and RotationTweenToggle
/// </summary>
public class TweenToggle : MonoBehaviour{

	protected bool isMoving;
	public bool IsMoving{
		get{ return isMoving; }
	}

	protected bool isShown;
	public bool IsShown{
		get{ return isShown; }
	}

	//////////////////////////////////////////////////////
	
	public bool isUsingDemultiplexer = false;
	public bool startsHidden = false;
	public float hideDeltaX; //Position, Scale, or Rotation depending on subclass
	public float hideDeltaY;
	public float hideDeltaZ;
	public float showDuration = 0.5f;
	public float hideDuration = 0.5f;
	public float showDelay = 0.0f;
	public float hideDelay = 0.0f;
	public LeanTweenType easeHide = LeanTweenType.easeInOutQuad;
	public LeanTweenType easeShow = LeanTweenType.easeInOutQuad;
	protected Vector3 hiddenPos;
	protected Vector3 showingPos;
	public bool isUseEstimatedTime = false;
	public Vector3 GetShowPos(){
		return showingPos;
	}

	protected bool positionSet;
	
	//////////////////////////////////////////////////////
	
	// Finish tween callback operations
	public GameObject ShowTarget;
	public string ShowFunctionName;
	public bool ShowIncludeChildren = false;
	public GameObject HideTarget;
	public string HideFunctionName;
	public bool HideIncludeChildren = false;
	
	//////////////////////////////////////////////////////
	
	// sounds to be played when this object tweens in or out.
	// we might want these to get fancier at some point (i.e. start the sound when the object tweens in, or when it finishes, etc)
	public string strSoundShow;
	public string strSoundHide;

	protected bool isGUI;				// Check if Unity GUI, will be set on awake
	protected RectTransform GUIRectTransform;	// Local cache of rect transform if GUI

	protected void Awake(){
		GUIRectTransform = gameObject.GetComponent<RectTransform>();
		if(GUIRectTransform != null){
			isGUI = true;
		}
		else{
			isGUI = false;
		}

		RememberPositions();
	}
	
	protected void Start(){
		Reset();
	}
	
	protected virtual void RememberPositions(){
		// Implement in child
	}

	public virtual void Reset(){
		// Implement in child
	}

	protected void ResetFinish() {
		gameObject.SetActive(!startsHidden);
	}

	//	void OnGUI(){
	//		if(isDebug){
	//			if(GUI.Button(new Rect(testButtonPos.x, testButtonPos.y, 100, 100), "show")){
	//				Show();
	//			}
	//			if(GUI.Button(new Rect(testButtonPos.x + 110, testButtonPos.y, 100, 100), "hide")){
	//				Hide();
	//			}
	//		}
	//	}

	public void Show(){
		gameObject.SetActive(true);
		Show(showDuration);
	}
	
	public virtual void Show(float time){
		// Implement in child
	}

	// Wait one frame and then show
	public void ShowAfterInit() {
		StartCoroutine(ShowAfterInitHelper());
	}

	private IEnumerator ShowAfterInitHelper() {
		yield return 0;
		Show();
	}
	
	// Since Hide() only saves its show/hide position in the beginning,
	// 	if something has a dynamically changing position (inventory),
	//	this will update the new positions before tweening
	public void HideWithUpdatedPosition(){
		if(isShown){
			RememberPositions();
			Hide();
		}
	}
	
	public void Hide(){
		Hide(hideDuration);
	}

	public virtual void Hide(float time){
		// Implement in child
	}

	// Wait one frame and then show
	public void HideAfterInit() {
		StartCoroutine(HideAfterInitHelper());
	}

	private IEnumerator HideAfterInitHelper() {
		yield return 0;
		Hide();
	}

	///////////////////////// CALLBACKS ///////////////////////////////
	protected void ShowSendCallback(){	
		isMoving = false;
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

	protected void HideSendCallback(){
		isMoving = false;
		if(string.IsNullOrEmpty(HideFunctionName)){
			// Toggle object off
			gameObject.SetActive(false);
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

		// Toggle object off
		gameObject.SetActive(false);
	}
}
