//// Copyright (c) 2015 LifeGuard Games Inc.

using UnityEngine;
using System.Collections;

/// <summary>
/// Position tween toggle.
/// Child of TweenToggle parent, Takes care of translation toggles
/// </summary>
public class PositionTweenToggle : TweenToggle {
	
	protected override void RememberPositions(){
		if(isGUI){
			showingPos = GUIRectTransform.anchoredPosition3D;
			hiddenPos = GUIRectTransform.anchoredPosition3D + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);

		}
		else{
			showingPos = gameObject.transform.localPosition;
			hiddenPos = gameObject.transform.localPosition + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
		}
	}

	public void UpdateHideDelta() {
		if(isShown) {
			if(isGUI) {
				hiddenPos = GUIRectTransform.anchoredPosition3D + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);

			}
			else {
				hiddenPos = gameObject.transform.localPosition + new Vector3(hideDeltaX, hideDeltaY, hideDeltaZ);
			}
		}
		else {
			Debug.LogError("delta hide changed while hidden");
		}
	}
	
	public override void Reset(){
		if(startsHidden){
			if(isGUI){
				GUIRectTransform.anchoredPosition3D = hiddenPos;
			}
			else{
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
			isShown = true;
			isMoving = true;
			
			LeanTween.cancel(gameObject);

			if(isGUI){
				LeanTween.move(GUIRectTransform, showingPos, time)
					.setEase(easeShow)
						.setDelay(showDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(ShowSendCallback);
			}
			else{
				LeanTween.moveLocal(gameObject, showingPos, time)
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
				LeanTween.move(GUIRectTransform, hiddenPos, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(HideSendCallback);
			}
			else{
				LeanTween.moveLocal(gameObject, hiddenPos, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
							.setUseEstimatedTime(isUseEstimatedTime)
								.setOnComplete(HideSendCallback);
			}
		}
	}
}
