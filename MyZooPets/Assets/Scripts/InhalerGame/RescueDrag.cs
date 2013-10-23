using UnityEngine;
using System.Collections;

/*
    Dragging the whole inhaler (step 5)
    Listens to drag. gameobject will be snapback if it doesn't land on the target collider
*/
public class RescueDrag : InhalerPart{
    public GameObject targetCollider; //Target position of the inhaler

    private Vector3 startDragPos; //Original position of the inhaler
    private Vector3 targetDragPos;
    private bool doneWithDrag = true;

    protected override void Awake(){
        gameStepID = 5;
        startDragPos = transform.position;
        targetDragPos = new Vector3(6.3f, 2.7f, 16.8f);
    }

    void OnDrag(DragGesture gesture){
       // current gesture phase (Started/Updated/Ended)
        ContinuousGesturePhase phase = gesture.Phase; 

        if(phase == ContinuousGesturePhase.Ended){ //Check where spacer has been dropped
            Ray ray = Camera.main.ScreenPointToRay(gesture.Position);
            RaycastHit hit ;
            bool snapBack = true;
            int maskLayer = 1 << 9;

            //Snap to position if spacer is at target position or revert to start pos
            if(Physics.Raycast(ray, out hit, 100, maskLayer)){
                if(hit.collider.gameObject == targetCollider){ 
                    transform.position = targetDragPos;
                    if(!doneWithDrag){
                        NextStep();
                        snapBack = false;
                    }
                }
            }

            if(snapBack){
                transform.position = startDragPos;
            }
        }
    }

    protected override void Enable(){
        transform.collider.enabled = true;
        targetCollider.SetActive(true);
        doneWithDrag = false;
    }

    protected override void NextStep(){
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerToMouth" );
		
        base.NextStep();
        Destroy(targetCollider);
        transform.collider.enabled = false;
        targetCollider.SetActive(true);
        doneWithDrag = true;
    }
}