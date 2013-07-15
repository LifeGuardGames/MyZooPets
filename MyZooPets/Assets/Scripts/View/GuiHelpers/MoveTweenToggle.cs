using UnityEngine;
using System.Collections;

/// <summary>
/// Move tween toggle.
/// Used to toogle move objects with LeanTween
/// </summary>

public class MoveTweenToggle : MonoBehaviour {

	private bool isActive = true;
	private bool isLocked = false;
	
	public Vector2 testButtonPos;
	public float showDeltaX;
	public float showDeltaY;
	public float hideDeltaX;
	public float hideDeltaY;
	public LeanTweenType easeOut;
	public LeanTweenType easeIn;

	void OnGUI(){
		if(GUI.Button(new Rect(testButtonPos.x, testButtonPos.y, 100, 100), "show")){
			Show();
		}
		if(GUI.Button(new Rect(testButtonPos.x + 110, testButtonPos.y, 100, 100), "hide")){
			Hide();
		}
	}
	
	public void Show(){
		if(!isActive && !isLocked){
			isActive = true;
			isLocked = true;
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeIn);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x + showDeltaX, 
				gameObject.transform.position.y + showDeltaY, gameObject.transform.position.z), 0.5f, optional);
		}
		else{
			Debug.LogError("trying show locked/active HUD");
		}
	}
	
	public void Hide(){
		if(isActive && !isLocked){
			isActive = false;
			isLocked = true;
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeOut);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "Unlock");		// Callback here
			LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x + hideDeltaX, 
				gameObject.transform.position.y + hideDeltaY, gameObject.transform.position.z), 0.5f, optional);
		}
		else{
			Debug.LogError("trying hide locked/inactive HUD");
		}
	}
	
	// Callback
	private void Unlock(){
		isLocked = false;
	}
}
