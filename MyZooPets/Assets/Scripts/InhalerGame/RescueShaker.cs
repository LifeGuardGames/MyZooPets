using UnityEngine;
using System.Collections;

/*
    Inhaler shaker (Step 3)
    Listens to drag gesture. Dragging is confined to a plan so it looks shaking the
    inhaler
*/
public class RescueShaker : InhalerPart {
    
    private Vector3 startDragPos;
    private bool doneWithShake = false; //disable shake after it's done

   protected override void Awake(){
        gameStepID = 3;
        startDragPos = transform.position;
   }

  void OnDrag(DragGesture gesture) { 
     // current gesture phase (Started/Updated/Ended)
      ContinuousGesturePhase phase = gesture.Phase; 

      if(phase == ContinuousGesturePhase.Ended){
          transform.position = startDragPos;
          if(doneWithShake) GetComponent<TBDragToMove>().DragPlaneCollider = null;
      }
      else if(phase == ContinuousGesturePhase.Updated){
          float elapsed = gesture.ElapsedTime; 

          if(!doneWithShake && elapsed >= 1.5f){ //Shake inhaler for 3 seconds
              NextStep();
          }
      }
  }
   
   protected override void Disable(){
        transform.collider.enabled = false;
   }

   protected override void Enable(){
        transform.collider.enabled = true;
   }

   protected override void NextStep(){
        base.NextStep();
		
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerShake" );		

        doneWithShake = true;
        Disable();
        transform.position = startDragPos;
   }
}