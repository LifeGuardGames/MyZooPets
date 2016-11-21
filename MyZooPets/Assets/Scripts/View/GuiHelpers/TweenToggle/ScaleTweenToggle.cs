//// Copyright (c) 2015 LifeGuard Games Inc.

using UnityEngine;
using System.Collections;

/// <summary>
/// Position tween toggle.
/// Child of TweenToggle parent, Takes care of translation toggles
/// </summary>
public class ScaleTweenToggle : TweenToggle {
	
	protected override void RememberPositions(){
		if(isGUI){
			showingPos = GUIRectTransform.localScale;
			hiddenPos = GUIRectTransform.localScale + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
		}
		else{
			showingPos = gameObject.transform.localScale;
			hiddenPos = gameObject.transform.localScale + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
		}
	}
	
	public override void Reset(){
		if (startsHidden){
			if(isGUI){
				GUIRectTransform.localScale = hiddenPos;
			}
			else{
				gameObject.transform.localScale = hiddenPos;
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
			isShown = true;
			isMoving = true;

            LeanTween.cancel(gameObject);

			if(isGUI){
				LeanTween.scale(GUIRectTransform, showingPos, time)
					.setEase(easeShow)
						.setDelay(showDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(ShowSendCallback);
			}
			else{
				LeanTween.scale(gameObject, showingPos, time)
					.setEase(easeShow)
						.setDelay(showDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(ShowSendCallback);
			}
		}
	}

	public override void Hide(float time){
		if(isShown){
			isShown = false;
			isMoving = true;
			
            LeanTween.cancel(gameObject);

			if(isGUI){
				LeanTween.scale(GUIRectTransform, hiddenPos, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(HideSendCallback);
			}
			else{
				LeanTween.scale(gameObject, hiddenPos, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(HideSendCallback);
			}
		}
	}
}
