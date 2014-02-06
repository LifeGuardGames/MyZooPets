using UnityEngine;
using System.Collections;

/*
    Step 6.
    Listens to pinch gesture. 
*/
public class RescuePrescription : InhalerPart{
    private float minGapDistance = 180.0f; //Pinch will only be registered if the 
                                        //gap distance is <= minGapDistance when the gesture ended

   protected override void Awake(){
        base.Awake();
        gameStepID = 6;
        floatyOptions.Add("text", Localization.Localize("INHALER_FLOATY_ROCKIT"));
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
    
#if UNITY_EDITOR
    void Update(){
       if(Input.GetKeyDown(KeyCode.P)){
            if(InhalerLogic.Instance.IsCurrentStepCorrect(gameStepID)){
                PrescriptionAnimation();
                NextStep();
            }
       }
    }
#endif

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
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerSqueeze" );				
		
        base.NextStep();
        Disable();
    }
}