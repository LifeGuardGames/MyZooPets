using UnityEngine;
using System.Collections;

/// <summary>
/// Tween toggle.
/// Used to toogle move objects with LeanTween
/// Parent class not to be used, implemented by PositionTweenToggle, ScaleTweenToggle, and RotationTweenToggle
/// </summary>
public class TweenToggle : MonoBehaviour {

	public bool IsMoving{ get { return isMoving; } }
	protected bool isMoving;
	public bool IsShowing{ get { return isShown; } }
	protected bool isShown;

	//////////////////////////////////////////////////////
	
	public bool ignoreTimeScale = false;
	public bool isUsingDemultiplexer = false;
	
	public bool blockUI = true;		// If true, when this object is tweening it will lock the UI
	public bool startsHidden = false;
	
	public float hideDeltaX; //Position, Scale, or Rotation depending on subclass
	public float hideDeltaY;
	public float hideDeltaZ;
	
	public float showDuration = 0.5f;
	public float hideDuration = 0.5f;
	public float showDelay = 0.0f;
	public float hideDelay = 0.0f;
	public LeanTweenType easeHide = LeanTweenType.easeInBack;
	public LeanTweenType easeShow = LeanTweenType.easeOutBack;

	protected Vector3 hiddenPos;
	protected Vector3 showingPos;
	public Vector3 GetShowPos() {
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
	
	// Testing purposes, isDebug true will show OnGUI buttons
	public bool isDebug = false;
	public Vector2 testButtonPos; // Set base positions of test buttons
	

	protected void Awake(){
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
		// play sound (if it exists)
		if ( !string.IsNullOrEmpty(strSoundShow) )
			AudioManager.Instance.PlayClip( strSoundShow );
		
		Show(showDuration);
	}

	public virtual void Show(float time){
		// Implement in child
	}

	public void Hide(){
		// play sound (if it exists)
		if ( !string.IsNullOrEmpty(strSoundHide) )
			AudioManager.Instance.PlayClip( strSoundHide );		
		
		Hide(hideDuration);
	}

	public virtual void Hide(float time){
		// Implement in child
	}

	///////////////////////// CALLBACKS ///////////////////////////////

	protected void ShowUnlockCallback(Hashtable hash){
		// Since LeanTween uses BroadcastMessage, it applies to all children, check for self object
		if(hash["selfCaller"] == this.gameObject){
//			print("HASH " + hash["selfCaller"] + " " + gameObject.name);
			// If this tween locks the UI, now that the tween is finished, decrement the counter
			if(blockUI){
				ClickManager.Instance.DecrementTweenCount();
			}
			isMoving = false;
			ShowSendCallback();
		}
	}

	protected void HideUnlockCallback(Hashtable hash){
		// Since LeanTween uses BroadcastMessage, it applies to all children, check for self object
		if(hash["selfCaller"] == this.gameObject){
			// If this tween locks the UI, now that the tween is finished, decrement the counter
			if(blockUI){
				ClickManager.Instance.DecrementTweenCount();
			}
			isMoving = false;
			HideSendCallback();
		}
	}

	protected void ShowSendCallback(){		
		if(string.IsNullOrEmpty(ShowFunctionName)){
			return;
		}
		if(ShowTarget == null){
			ShowTarget = gameObject;
		}
		if(ShowIncludeChildren){
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

	protected void HideSendCallback(){
		if(string.IsNullOrEmpty(HideFunctionName)){
			return;
		}
		if(HideTarget == null){
			HideTarget = gameObject;
		}
		if(HideIncludeChildren){
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
