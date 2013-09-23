using UnityEngine;
using System;
using System.Collections;

/*
    Rescue Space (Rescue Step 2)
    Listens to Drag gesture. If drag doesn't end on the target collider the spacer
    will be snapped back to its starting position
*/
public class RescueSpacer : InhalerPart{
    public GameObject rescueSpacerTarget; //Target position of the spacer
    public GameObject rescueBody; //Attach spacer to body

    private Vector3 startDragPos; //Original position of the spacer
    private Vector3 targetDragPos;

    protected override void Awake(){
        gameStepID = 2;
        startDragPos = transform.position;
        targetDragPos = new Vector3(8, -3, 13);
    }

    void OnDrag(DragGesture gesture) { 
       // current gesture phase (Started/Updated/Ended)
        ContinuousGesturePhase phase = gesture.Phase; 

        if(phase == ContinuousGesturePhase.Ended){ //Check where spacer has been dropped
            Ray ray = Camera.main.ScreenPointToRay(gesture.Position);
            RaycastHit hit ;
            bool snapBack = true;
            int maskLayer = 1 << 9;


            //Snap to position if spacer is at target position or revert to start pos
            if(Physics.Raycast(ray, out hit, 100, maskLayer)){
                if(hit.collider.gameObject == rescueSpacerTarget){ 
                    transform.position = targetDragPos;
                    transform.parent = rescueBody.transform;

                    NextStep();
                    snapBack = false;
                }
            }

            if(snapBack){
                transform.position = startDragPos;
            }
        }
    }

    protected override void Disable(){
        gameObject.SetActive(false);
        rescueSpacerTarget.SetActive(false);
    }

    protected override void Enable(){
        gameObject.SetActive(true);
        rescueSpacerTarget.SetActive(true);
    }

    protected override void NextStep(){
        base.NextStep();
        Destroy(rescueSpacerTarget);
        transform.collider.enabled = false;
    }
}