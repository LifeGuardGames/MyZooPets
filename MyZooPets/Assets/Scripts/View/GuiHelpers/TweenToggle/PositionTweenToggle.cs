using UnityEngine;
using System.Collections;

/// <summary>
/// Position tween toggle.
/// Child of TweenToggle parent, Takes care of translation toggles
/// </summary>
public class PositionTweenToggle : TweenToggle {
	
	protected override void RememberPositions(){
		showingPos = gameObject.transform.localPosition;
		hiddenPos = gameObject.transform.localPosition + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
	}
	
	public override void Reset(){
		if (startsHidden){
			if (positionSet){ // If not, Reset() will be called again by Awake() later
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
	
	public override void Show(float time){
		if(!isShown){
			// If this tween locks the UI, properly increment the counter
			if ( bLockUI )
				ClickManager.Instance.IncrementTweenCount();
			
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

	public override void Hide(float time){
		if(isShown){
			// If this tween locks the UI, properly increment the counter
			if ( bLockUI )
				ClickManager.Instance.IncrementTweenCount();
			
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
}
