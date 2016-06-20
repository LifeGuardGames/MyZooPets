using UnityEngine;
using System.Collections;

/*
    Step 6.
    Listens to pinch gesture. 
*/
public class RescuePrescription : InhalerPart{
//    private float minGapDistance = 220.0f; //Pinch will only be registered if the 
                                        //gap distance is <= minGapDistance when the gesture ended

   protected override void Awake(){
        base.Awake();
        gameStepID = 6;
        floatyOptions.Add("text", Localization.Localize("INHALER_FLOATY_ROCKIT"));
   }

	void OnTap(TapGesture gesture){
		PrescriptionAnimation();
		NextStep();
	}

//    void OnPinch(PinchGesture gesture){ 
//        // current gesture phase (Started/Updated/Ended)
//        ContinuousGesturePhase phase = gesture.Phase;
//
//        if(phase == ContinuousGesturePhase.Ended){
//            // Current gap distance between the two fingers
//            float gap = gesture.Gap;
//				NOTE: gap is in pixel. need to be change to an adjusted unit so it
	//			 stays the same across all devices
//            if(gap <= minGapDistance){
//                PrescriptionAnimation();
//                NextStep();
//            }
//        }
//    }
    
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
        Vector3 to = new Vector3(transform.localPosition.x, 5.8f, transform.localPosition.z);
        LeanTween.moveLocal(gameObject, to, 0.5f);
    }

    protected override void Disable(){
        transform.GetComponent<Collider>().enabled = false;
    }

    protected override void Enable(){
        transform.GetComponent<Collider>().enabled = true;
    }

    protected override void NextStep(){
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerSqueeze" );				
		
        base.NextStep();
        Disable();
    }
}