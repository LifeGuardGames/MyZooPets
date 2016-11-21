using UnityEngine;
using System;

public class Exhale : InhalerPart {
    protected void Awake(){
        gameStepID = 4;
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 
       if(direction == FingerGestures.SwipeDirection.Right){
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

	void OnDrag(DragGesture gesture){
		if(!isGestureRecognized){
			Vector3 begin = new Vector3 (0,0,0);
			Vector3 ended;
			if(gesture.Phase == ContinuousGesturePhase.Ended){
				ended = gesture.Position;
				begin = gesture.StartPosition;
				if(begin.x < ended.x){
					isGestureRecognized = true;
					
					//Disable hint when swipe gesture is registered. 
					GetComponent<HintController>().DisableHint(false);
					
					AudioManager.Instance.PlayClip("inhalerExhale");
					LgInhalerAnimationEventHandler.BreatheOutEndEvent += BreatheOutEndEventHandler;	
					petAnimator.SetTrigger("BreatheOut");
				}
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
