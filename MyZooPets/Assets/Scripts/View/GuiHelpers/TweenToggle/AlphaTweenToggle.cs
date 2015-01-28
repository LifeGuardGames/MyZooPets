using UnityEngine;
using System.Collections;

public class AlphaTweenToggle : TweenToggle {

	////////////////////////////////////////////////////////////
	// Put additional supported renderers and shaders here
	public bool isSprite = true;
	private SpriteRenderer spriteRenderer;
	////////////////////////////////////////////////////////////

	public float hideDeltaAlpha = 1f;

	private float showingAlpha;
	private float hiddenAlpha;

	

	protected override void RememberPositions(){
		// Sanitize the input
		if(hideDeltaAlpha > 1f){
			Debug.LogWarning("Hide delta is greater than 1");
			hideDeltaAlpha = 1f;
		}

		////////////////////////////////////////
		// Assign all the other ones false here
		////////////////////////////////////////
		if(isSprite){
			spriteRenderer = GetComponent<SpriteRenderer>();
			showingAlpha = spriteRenderer.color.a;
			hiddenAlpha = showingAlpha - hideDeltaAlpha;
		}
	}
	
	public override void Reset(){
		if (startsHidden){
			Debug.Log("STARTS HIDDEN");
			if(isSprite){
				spriteRenderer.color = new Color(spriteRenderer.color.r,
				                                 spriteRenderer.color.g,
				                                 spriteRenderer.color.b,
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
			// If this tween locks the UI, properly increment the counter
			if(blockUI){
				ClickManager.Instance.IncrementTweenCount();
				
				// Already tweening, and want to abort and decrement
				if(isMoving){
					ClickManager.Instance.DecrementTweenCount();
				}
			}
			
			isShown = true;
			isMoving = true;
			
			Hashtable completeParamHash = new Hashtable();
			completeParamHash.Add("selfCaller", gameObject);
			
			LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeShow);
			optional.Add("delay", showDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onCompleteParam", completeParamHash);
			optional.Add("onComplete", "ShowUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}
			LeanTween.alpha(gameObject, showingAlpha, time, optional);
		}
		else{
			Debug.LogWarning("Alpha tween toggle show is in bad state to call show");
		}
	}
	
	public override void Hide(float time){
		Debug.Log("HIDING is shown: " + isShown);
		if(isShown){
			// If this tween locks the UI, properly increment the counter
			if(blockUI){
				ClickManager.Instance.IncrementTweenCount();
				
				// Already tweening, and want to abort and decrement
				if(isMoving){
					ClickManager.Instance.DecrementTweenCount();
				}
			}

			Debug.Log("CALLING THIS SHIT");

			isShown = false;
			isMoving = true;
			
			Hashtable completeParamHash = new Hashtable();
			completeParamHash.Add("selfCaller", gameObject);
			
			LeanTween.cancel(gameObject);
			Hashtable optional = new Hashtable();
			optional.Add("ease", easeHide);
			optional.Add("delay", hideDelay);
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onCompleteParam", completeParamHash);
			optional.Add("onComplete", "HideUnlockCallback");
			if (ignoreTimeScale){
				optional.Add("useEstimatedTime", true);
			}
			LeanTween.alpha(gameObject, hiddenAlpha, time, optional);
		}
		else{
			Debug.LogWarning("Alpha tween toggle show is in bad state to call hide");
		}
	}
}
