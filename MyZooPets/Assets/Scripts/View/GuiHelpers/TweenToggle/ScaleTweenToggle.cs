using UnityEngine;
using System.Collections;

/// <summary>
/// Position tween toggle.
/// Child of TweenToggle parent, Takes care of translation toggles
/// </summary>
public class ScaleTweenToggle : TweenToggle {
	
	protected override void RememberPositions(){
		showingPos = gameObject.transform.localScale;
		hiddenPos = gameObject.transform.localScale + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
	}
	
	public override void Reset(){
		if (startsHidden){
			gameObject.transform.localScale = hiddenPos;
			
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
			if(blockUI){
				ClickManager.Instance.IncrementTweenCount();
			}
				
			isShown = true;
			isMoving = true;

            LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();

			Hashtable completeParamHash = new Hashtable();
			completeParamHash.Add("selfCaller", gameObject);
			
			optional.Add("ease", easeShow);
			optional.Add("delay", showDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onCompleteParam", completeParamHash);
			optional.Add("onComplete", "ShowUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}

			LeanTween.scale(gameObject, showingPos, time, optional);
		}
	}

	public override void Hide(float time){
		if(isShown){
			// If this tween locks the UI, properly increment the counter
			if(blockUI){
				ClickManager.Instance.IncrementTweenCount();
			}
			
			isShown = false;
			isMoving = true;
			
            LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();
			
			Hashtable completeParamHash = new Hashtable();
			completeParamHash.Add("selfCaller", gameObject);
			
			optional.Add("ease", easeHide);
			optional.Add("delay", hideDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onCompleteParam", completeParamHash);
			optional.Add("onComplete", "HideUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}
			LeanTween.scale(gameObject, hiddenPos, time, optional);
		}
	}
}
