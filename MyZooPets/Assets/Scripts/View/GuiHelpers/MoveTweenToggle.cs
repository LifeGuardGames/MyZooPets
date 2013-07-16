using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle.
/// Used to toogle move objects with LeanTween
/// </summary>

public class MoveTweenToggle : MonoBehaviour {

	private bool isActive;
	private bool isLocked;

	public bool isDebug = false;
	public bool startsHidden = false; //True: the UI element started off the screen so Show function
										//will need to be called first, False: UI element started off
										//displayed on screen so Hide function need to be called first
	public Vector2 testButtonPos; //position of the test buttons
	public float showDeltaX;
	public float showDeltaY;
	public float hideDeltaX;
	public float hideDeltaY;
	public LeanTweenType easeHide;
	public LeanTweenType easeShow;


	void Awake(){
		if (startsHidden){
			gameObject.transform.position = new Vector3(
				gameObject.transform.position.x + hideDeltaX,
				gameObject.transform.position.y + hideDeltaY,
				gameObject.transform.position.z
			);
		}

		if(startsHidden){ //need to call show first
			isActive = false;
			isLocked = false;
		}else{ //need to call hide first
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
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x + showDeltaX,
				gameObject.transform.position.y + showDeltaY, gameObject.transform.position.z), time, optional);
		}
		// else{
		// 	Debug.LogError("trying show locked/active HUD");
		// }
	}

	public void Show(){
		Show(0.5f);
	}

	public void Hide(float time){
		if(isActive && !isLocked){
			isActive = false;
			isLocked = true;
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeHide);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x + hideDeltaX,
				gameObject.transform.position.y + hideDeltaY, gameObject.transform.position.z), time, optional);
		}
		// else{
		// 	Debug.LogError("trying hide locked/inactive HUD");
		// }
	}

	public void Hide(){
		Hide(0.5f);
	}

	// Callback
	private void Unlock(){
		print("callback");
		isLocked = false;
	}
}
