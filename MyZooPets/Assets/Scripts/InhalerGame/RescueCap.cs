using UnityEngine;
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
                //Lean tween cap
                Vector3 to = new Vector3(2, -6, 0); //off the screen
                Hashtable optional = new Hashtable();
                optional.Add("onCompleteTarget", gameObject);
                optional.Add("onComplete", "NextStep");
                LeanTween.move(gameObject, to, 0.5f, optional);
            }
        }
    }

    protected override void Enable(){
        gameObject.SetActive(true);
    }

    protected override void NextStep(){
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerRemoveCap" );

        gameObject.SetActive(false);

       base.NextStep();
    }
}