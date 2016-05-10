using UnityEngine;
using System;
using System.Collections;

/*
    Rescue Inhaler Cap (Rescue Step 1).
    Listens to swipe gesture from FingerGesture.
*/
public class RescueCap : InhalerPart{
    protected override void Awake(){
        base.Awake();
        gameStepID = 1;
    }

    void OnSwipe(SwipeGesture gesture) { 
       FingerGestures.SwipeDirection direction = gesture.Direction; 
       if(direction == FingerGestures.SwipeDirection.Left || 
            direction == FingerGestures.SwipeDirection.LowerLeftDiagonal){

            //If current step is the right sequence
            if(InhalerLogic.Instance.IsCurrentStepCorrect(gameStepID)){

				if(!isGestureRecognized){
					isGestureRecognized = true;

					//Lean tween cap
					Vector3 to = new Vector3(2, -6, 0); //off the screen
					LeanTween.move(gameObject, to, 0.5f).setOnComplete(NextStep);
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