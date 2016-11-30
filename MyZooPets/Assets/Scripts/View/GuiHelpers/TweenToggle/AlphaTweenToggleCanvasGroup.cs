using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AlphaTweenToggleCanvasGroup : TweenToggle {
	public float hideDeltaAlpha;

	private float showingAlpha;
	private float hiddenAlpha;
	private CanvasGroup canvasGroup;

	protected override void RememberPositions() {
		canvasGroup = GUIRectTransform.GetComponent<CanvasGroup>();
        showingAlpha = canvasGroup.alpha;
		hiddenAlpha = showingAlpha - hideDeltaAlpha;

		// Sanity check on hidden alpha values, sometimes we want to override simply
		if(hiddenAlpha < 0) {
			hiddenAlpha = 0f;
		}
		else if(hiddenAlpha > 1) {
			Debug.LogWarning("Hidden alpha is out of bounds: " + hiddenAlpha);
			hiddenAlpha = 1f;
		}
	}

	public override void Reset() {
		if(startsHidden) {
			GUIRectTransform.GetComponent<CanvasGroup>().alpha = hiddenAlpha;
			
			// Need to call show first
			isShown = false;
			isMoving = false;
		}
		else {
			// Need to call hide first
			isShown = true;
			isMoving = false;
		}

		ResetFinish();
	}

	public override void Show(float time) {
		if(!isShown) {
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			isShown = true;
			isMoving = true;

			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject, SetAlpha, hiddenAlpha, showingAlpha, time)
				.setEase(easeShow)
					.setDelay(showDelay)
					.setUseEstimatedTime(isUseEstimatedTime)
					.setOnComplete(ShowSendCallback);
		}
	}

	public override void Hide(float time) {
		if(isShown) {
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			isShown = false;
			isMoving = true;

			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject, SetAlpha, showingAlpha, hiddenAlpha, time)
				.setEase(easeHide)
					.setDelay(hideDelay)
					.setUseEstimatedTime(isUseEstimatedTime)
					.setOnComplete(HideSendCallback);
		}
	}

	private void SetAlpha(float value) {
		GUIRectTransform.GetComponent<CanvasGroup>().alpha = value;
	}
}
