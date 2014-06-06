using UnityEngine;
using System;
using System.Collections;

/*
    Handles inhale (swipe up) action
*/
public class Inhale : InhalerPart{
	public InhalerAnimationController animationController;

	protected override void Awake(){
		base.Awake();
		gameStepID = 7;
		floatyOptions.Add("text", ""); 
	}

	void OnSwipe(SwipeGesture gesture){
		FingerGestures.SwipeDirection direction = gesture.Direction; 

		if(direction == FingerGestures.SwipeDirection.Up){

			if(InhalerAnimationController.OnAnimDone == null){


				//Disable hint when swipe gesture is registered. 
				GetComponent<HintController>().DisableHint(false);

				animationController.Inhale();
				AudioManager.Instance.PlayClip("inhalerInhale"); 

				//using invoke instead of listening to animationController callback
				//because LWFAnimator sometimes sends callback prematurely. Don't
				//want to debug LWFAnimator since we are switching away from it soon
				Invoke("NextStep", 3.5f);
			}
		}
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
