//// Copyright (c) 2015 LifeGuard Games Inc.

using UnityEngine;
using System.Collections;

/// <summary>
/// Rotation tween toggle.
/// Child of TweenToggle parent, Takes care of rotation toggles
/// </summary>
public class RotationTweenToggle : TweenToggle {

	protected override void RememberPositions(){
		if(isGUI){
			showingPos = GUIRectTransform.localEulerAngles;
			hiddenPos = GUIRectTransform.localEulerAngles + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
		}
		else{
			showingPos = gameObject.transform.localEulerAngles;
			hiddenPos = gameObject.transform.localEulerAngles + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
		}
	}
	
	public override void Reset(){
		if (startsHidden){
			if(isGUI){
				GUIRectTransform.localEulerAngles = hiddenPos;
			}
			else{
				gameObject.transform.localEulerAngles = hiddenPos;
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
				if(showingPos.x != 0f || showingPos.z != 0f){
					Debug.LogWarning("GUI rotation will only rotate z-axis");
				}
				LeanTween.rotate(GUIRectTransform, showingPos.z, time)
					.setEase(easeShow)
						.setDelay(showDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(ShowSendCallback);
			}
			else{
				LeanTween.rotateLocal(gameObject, showingPos, time)
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
				if(showingPos.x != 0f || showingPos.z != 0f){
					Debug.LogWarning("GUI rotation will only rotate z-axis");
				}
				LeanTween.rotate(GUIRectTransform, hiddenPos.z, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(ShowSendCallback);
			}
			else{
				LeanTween.rotateLocal(gameObject, hiddenPos, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(HideSendCallback);
			}
		}
	}
}
