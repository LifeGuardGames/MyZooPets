using UnityEngine;
using UnityEngine.UI;

public class AlphaTweenToggle : TweenToggle {
	public float hideDeltaAlpha;

	private float showingAlpha;
	private float hiddenAlpha;
	private SpriteRenderer spriteRenderer;

	protected override void RememberPositions(){
		if(isGUI){
			showingAlpha = GUIRectTransform.GetComponent<Image>().color.a;
			hiddenAlpha = showingAlpha - hideDeltaAlpha;
		}
		else{
			// Assume SpriteRenderer is default
			spriteRenderer = GetComponent<SpriteRenderer>();
			showingAlpha = spriteRenderer.color.a;
			hiddenAlpha = showingAlpha - hideDeltaAlpha;
		}

		// Sanity check on hidden alpha values, sometimes we want to override simply
		if(hiddenAlpha < 0){
			hiddenAlpha = 0f;
		}
		else if(hiddenAlpha > 1){
			Debug.LogWarning("Hidden alpha is out of bounds: " + hiddenAlpha);
			hiddenAlpha = 1f;
		}
	}
	
	public override void Reset(){
		if(startsHidden){
			if(isGUI){
				Color colorAux = GUIRectTransform.GetComponent<Image>().color;
				GUIRectTransform.GetComponent<Image>().color = new Color(colorAux.r, colorAux.g, colorAux.b, hiddenAlpha);
			}
			else{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
												 hiddenAlpha);
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
				LeanTween.alpha(GUIRectTransform, showingAlpha, time)
					.setEase(easeShow)
						.setDelay(showDelay)
						.setUseEstimatedTime(isUseEstimatedTime)
						.setOnComplete(ShowSendCallback);
			}
			else{
				LeanTween.alpha(gameObject, showingAlpha, time)
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
				LeanTween.alpha(GUIRectTransform, hiddenAlpha, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
						.setUseEstimatedTime(isUseEstimatedTime)
						.setOnComplete(HideSendCallback);
			}
			else{
				LeanTween.alpha(gameObject, hiddenAlpha, time)
					.setEase(easeHide)
						.setDelay(hideDelay)
						.setUseEstimatedTime(isUseEstimatedTime)
						.setOnComplete(HideSendCallback);
			}
		}
	}
}
