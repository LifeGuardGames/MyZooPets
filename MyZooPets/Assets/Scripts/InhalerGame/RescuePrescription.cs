using UnityEngine;
using System.Collections;

/*
    Step 6.
    Listens to pinch gesture. 
*/
public class RescuePrescription : InhalerPart{
    private float minGapDistance = 80.0f; //Pinch will only be registered if the 
                                        //gap distance is <= minGapDistance when the gesture ended

   protected override void Awake(){
        gameStepID = 6;
   } 

    void OnPinch(PinchGesture gesture){ 
        // current gesture phase (Started/Updated/Ended)
        ContinuousGesturePhase phase = gesture.Phase;

        if(phase == ContinuousGesturePhase.Ended){
            // Current gap distance between the two fingers
            float gap = gesture.Gap;

            if(gap <= minGapDistance){
                PrescriptionAnimation();
                NextStep();
            }
        }
    }

    private void PrescriptionAnimation(){
        Vector3 to = new Vector3(transform.localPosition.x, 1.5f, transform.localPosition.z);
        LeanTween.moveLocal(gameObject, to, 0.5f, new Hashtable());
    }

    protected override void Disable(){
        transform.collider.enabled = false;
    }

    protected override void Enable(){
        transform.collider.enabled = true;
    }

    protected override void NextStep(){
        base.NextStep();
        Disable();
    }
}