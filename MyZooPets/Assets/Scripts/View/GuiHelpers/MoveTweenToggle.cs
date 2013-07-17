using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle.
/// Used to toogle move objects with LeanTween
/// </summary>

public class MoveTweenToggle : MonoBehaviour {
	
	public bool isUsingDemultiplexer = false;
	private bool isActive;
	private bool isLocked;
	public bool IsLocked{
		get{
			return isLocked;
		}
	}

	public bool isDebug = false;
	public bool startsHidden = false; 	//True: the UI element started off the screen so Show function
											//will need to be called first, False: UI element started off
											//displayed on screen so Hide function need to be called first
	public Vector2 testButtonPos; 		//position of the test buttons
	public float showDeltaX;
	public float showDeltaY;
	public float hideDeltaX;
	public float hideDeltaY;
	public float showDuration = 0.5f;
	public float hideDuration = 0.5f;
	public float showDelay = 0.0f;
	public float hideDelay = 0.0f;
	public LeanTweenType easeHide;
	public LeanTweenType easeShow;
	
	void Awake(){
		Reset();
	}

	public void Reset(){
		if (startsHidden){
			gameObject.transform.localPosition = new Vector3(
				gameObject.transform.localPosition.x + hideDeltaX,
				gameObject.transform.localPosition.y + hideDeltaY,
				gameObject.transform.localPosition.z
			);
		 	// Need to call show first
			isActive = false;
			isLocked = false;
		}
		else{
			// Need to call hide first
			isActive = true;
			isLocked = false;
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

	public void Show(float time){
		if(!isActive && !isLocked){
			print("tween");
			isActive = true;
			isLocked = true;
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeShow);
			optional.Add("delay", showDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.moveLocal(gameObject, new Vector3(gameObject.transform.localPosition.x + showDeltaX,
				gameObject.transform.localPosition.y + showDeltaY, gameObject.transform.localPosition.z), time, optional);
		}
		// else{
		// 	Debug.LogError("trying show locked/active HUD");
		// }
		
	}

	public void Show(){
		Show(showDuration);
	}

	public void Hide(float time){
		if(isActive && !isLocked){
			isActive = false;
			isLocked = true;
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeHide);
			optional.Add("delay", hideDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.moveLocal(gameObject, new Vector3(gameObject.transform.localPosition.x + hideDeltaX,
				gameObject.transform.localPosition.y + hideDeltaY, gameObject.transform.localPosition.z), time, optional);
		}
		// else{
		// 	Debug.LogError("trying hide locked/inactive HUD");
		// }
	}

	public void Hide(){
		Hide(hideDuration);
	}

	// Callback
	private void Unlock(){
		isLocked = false;
	}
}
