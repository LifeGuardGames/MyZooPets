using UnityEngine;
using System;
using System.Collections;

/*
    Handles exhale (Step 4)
    Listens to swipe gesture. 
*/
public class Exhale : InhalerPart {
//    public InhalerAnimationController animationController;
//	public Animator animator;

    protected override void Awake(){
        base.Awake();
        gameStepID = 4;
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Down){

       		if(!isGestureRecognized){
				isGestureRecognized = true;

				//Disable hint when swipe gesture is registered. 
				GetComponent<HintController>().DisableHint(false);

				AudioManager.Instance.PlayClip("inhalerExhale");
				LgInhalerAnimationEventHandler.BreatheOutEndEvent += BreatheOutEndEventHandler;	
				petAnimator.SetTrigger("BreatheOut");
			}
       }
    }

	private void BreatheOutEndEventHandler(object sender, EventArgs args){
		LgInhalerAnimationEventHandler.BreatheOutEndEvent -= BreatheOutEndEventHandler;
		NextStep();
	}

    protected override void Disable(){
        gameObject.SetActive(false);
    }

    protected override void Enable(){
        gameObject.SetActive(true);
    }

    protected override void NextStep(){
        base.NextStep();
        Destroy(gameObject);
    }
	
}
