using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle.
/// Used to toogle move objects with LeanTween
/// </summary>

public class MoveTweenToggle : MonoBehaviour {

	public bool ignoreTimeScale = false;
	public bool isUsingDemultiplexer = false;
	private bool isShown;
	private bool isMoving;
	public bool IsMoving{
		get{
			return isMoving;
		}
	}

	public bool isDebug = false;
	public bool startsHidden = false; 	//True: the UI element started off the screen so Show function
											//will need to be called first, False: UI element started off
											//displayed on screen so Hide function need to be called first
	public Vector2 testButtonPos; 		//position of the test buttons
	public float hideDeltaX;
	public float hideDeltaY;
	public float showDuration = 0.5f;
	public float hideDuration = 0.5f;
	public float showDelay = 0.0f;
	public float hideDelay = 0.0f;
	public LeanTweenType easeHide;
	public LeanTweenType easeShow;

	private Vector3 hiddenPos;
	private Vector3 showingPos;
	private bool positionSet;

	// Finish tween callback operations
	public GameObject ShowTarget;
	public string ShowFunctionName;
	public bool ShowIncludeChildren = false;

	public GameObject HideTarget;
	public string HideFunctionName;
	public bool HideIncludeChildren = false;

	void Awake(){
		RememberPositions();
		//Debug.Log("toggle awake");
		Reset();
	}

	void RememberPositions(){
		showingPos = gameObject.transform.localPosition;
		hiddenPos = gameObject.transform.localPosition + new Vector3(hideDeltaX, hideDeltaY, 0);
		positionSet = true;
	}

	public void Reset(){

		if (startsHidden){
			if (positionSet){ // if not, Reset() will be called again by Awake() later
				gameObject.transform.localPosition = hiddenPos;
			}
		 	// Need to call show first
			isShown = false;
			isMoving = false;
		}
		else{
			// Need to call hide first
			isShown = true;
			isMoving = false;
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

	public void Show(){
		Show(showDuration);
	}

	public void Show(float time){
		if(!isShown){
			isShown = true;
			isMoving = true;
            LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeShow);
			optional.Add("delay", showDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "ShowUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}
			LeanTween.moveLocal(gameObject, showingPos, time, optional);
		}
	}

	public void Hide(){
		Hide(hideDuration);
	}

	public void Hide(float time){
		if(isShown){
			isShown = false;
			isMoving = true;
            LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeHide);
			optional.Add("delay", hideDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "HideUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}
			LeanTween.moveLocal(gameObject, hiddenPos, time, optional);
		}
	}

	///////////////////////// CALLBACKS ///////////////////////////////

	private void ShowUnlockCallback(){
		isMoving = false;
		ShowSendCallback();
	}

	private void HideUnlockCallback(){
		isMoving = false;
		HideSendCallback();
	}

	private void ShowSendCallback(){
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

	private void HideSendCallback(){
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
