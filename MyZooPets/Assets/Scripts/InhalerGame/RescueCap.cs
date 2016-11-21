using UnityEngine;
using System;

/// <summary>
/// Rescue Inhaler Cap (Rescue Step 1)
/// Listens to swipe gesture from FingerGesture
/// </summary>
public class RescueCap : InhalerPart{
	public Vector3 targetPositionTween;

    protected void Awake(){
        gameStepID = 1;
    }

    void OnSwipe(SwipeGesture gesture) { 
       FingerGestures.SwipeDirection direction = gesture.Direction; 
       if(direction == FingerGestures.SwipeDirection.Left || 
            direction == FingerGestures.SwipeDirection.LowerLeftDiagonal){

            //If current step is the right sequence
            if(InhalerGameManager.Instance.IsCurrentStepCorrect(gameStepID)){

				if(!isGestureRecognized){
					isGestureRecognized = true;

					//Lean tween cap
					LeanTween.moveLocal(gameObject, targetPositionTween, 0.5f)
						.setEase(LeanTweenType.easeOutQuad)
						.setOnComplete(NextStep);
				}
            }
        }
    }

    protected override void Enable(){
        gameObject.SetActive(true);
    }

    protected override void NextStep(){
		// play sound here
		AudioManager.Instance.PlayClip("inhalerRemoveCap");
//		LgInhalerAnimationEventHandler.InhalerHappy1EndEvent += InhalerHappy1EndEventHandler;
		petAnimator.SetTrigger("InhalerHappy1");

		gameObject.SetActive(false);
		base.NextStep();
    }

	private void InhalerHappy1EndEventHandler(object sender, EventArgs args){
		LgInhalerAnimationEventHandler.InhalerHappy1EndEvent -= InhalerHappy1EndEventHandler;

	}
}