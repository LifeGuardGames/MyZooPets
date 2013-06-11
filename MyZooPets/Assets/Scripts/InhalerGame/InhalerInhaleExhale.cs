using UnityEngine;
using System.Collections;

public class InhalerInhaleExhale : MonoBehaviour {

    tk2dAnimatedSprite arrows;
    float firstTouchYPos;
    bool firstTouchOnObject = false;
    bool completedGame = false;
    bool pointingUp = false;
    NotificationUIManager notificationUIManager;
	// Use this for initialization
	void Start () {
        notificationUIManager = GameObject.Find("NotificationUIManager").GetComponent<NotificationUIManager>();
        arrows = GetComponent<tk2dAnimatedSprite>();
        renderer.enabled = false;
	}

	// Update is called once per frame
	void Update () {
        if (completedGame){
            return;
        }
        if (InhalerLogic.CurrentStep == 3){
            renderer.enabled = true;
        }

        if (InhalerLogic.CurrentStep == 5 && !pointingUp){
            arrows.FlipY();
            pointingUp = true;
            renderer.enabled = true;
        }

        if (Input.touchCount == 0) { // if not touching screen
            ResetTouch();
        }
        else if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (Input.GetMouseButtonDown(0) && isTouchingObject(touch)) {
                firstTouchOnObject = true;
                firstTouchYPos = touch.position.y;
            }
            else if (Input.GetMouseButton(0) && firstTouchOnObject) {
                if (isDraggingDown(touch)){
                    // check if step 3 is correct
                    // if it is, increment InhalerLogic.CurrentStep
                    if (InhalerLogic.IsCurrentStepCorrect(3)){
                        Debug.Log("Completed step 3");
                        InhalerLogic.IsDoneWithGame();
                        renderer.enabled = false;
                    }
                }
                else if (isDraggingUp(touch)){
                    // check if step 5 is correct
                    // if it is, increment InhalerLogic.CurrentStep
                    if (InhalerLogic.IsCurrentStepCorrect(5)){
                        Debug.Log("Completed step 5");
                        completedGame = true;
                        notificationUIManager.PopupTexture("great", null);
                        renderer.enabled = false;
                        InhalerLogic.IsDoneWithGame();
                    }
                }
            }
        }
	}

    void ResetTouch(){
        firstTouchOnObject = false;
    }

    bool isDraggingUp(Touch touch){
        if (touch.position.y - firstTouchYPos > 50){
            Debug.Log("is dragging up");
            return true;
        }
        return false;
    }
    bool isDraggingDown(Touch touch){
        if (firstTouchYPos - touch.position.y > 50){
            Debug.Log("is dragging down");
            return true;
        }
        return false;
    }

    bool isTouchingObject(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;

        //Check if there is a collider attached already, otherwise add one on the fly
        if(collider == null){
            gameObject.AddComponent(typeof(BoxCollider));
        }

        if (Physics.Raycast (ray, out hit)) {
            if(hit.collider.gameObject == this.gameObject){
                return true;
            }
        }
        return false;
    }
}
