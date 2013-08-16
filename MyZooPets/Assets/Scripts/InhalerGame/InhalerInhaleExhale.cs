using UnityEngine;
using System.Collections;

/*
    Inhale and Exhale gestures (Advair Steps 3 and 5 / Rescue Steps 3 and 6).

    This listens to the user's touch input, and "becomes" a smaller rescue inhaler when dragged
    to the pet.

    How this happens is, when the user's touch starts on the inhaler, the original body is hidden.
    At the same time, a GUI texture is drawn under the user's touch position.

    If the inhaler is dragged to and released over the pet, it will disappear, and a smaller
    rescue inhaler will appear next to the pet's head. Also, the step is completed.
    If it is released anywhere else, the original advair inhaler will reappear.
*/
public class InhalerInhaleExhale : MonoBehaviour {

    public AudioSource inhale;
    public AudioSource exhale;
    tk2dSpriteAnimator arrows;
    public GameObject breatheMessageObj;
    float firstTouchYPos;
    bool firstTouchOnObject = false;
    bool completedGame = false;
    bool pointingUp = false;
    int breathingInStep;
    public InhalerGameManager inhalerGameManager;
	// Use this for initialization
	void Start () {
        inhalerGameManager = GameObject.Find("InhalerGameManager").GetComponent<InhalerGameManager>();
        arrows = GetComponent<tk2dSpriteAnimator>();
        Disable();

        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair){
            breathingInStep = 5;
        }
        else if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            breathingInStep = 6;
        }
	}

	// Update is called once per frame
	void Update () {
        if (completedGame){
            return;
        }
        if (InhalerLogic.Instance.CurrentStep == breathingInStep && !pointingUp){
            arrows.Play("UpArrow");
            pointingUp = true;
            breatheMessageObj.GetComponent<tk2dSpriteAnimator>().Play("Breathe In");
        }

        CheckAndEnable();

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
                    // if it is, increment InhalerLogic.Instance.CurrentStep
                    if (InhalerLogic.Instance.IsCurrentStepCorrect(3)){
                        exhale.Play();
                        Debug.Log("Completed step 3");
                        InhalerLogic.Instance.NextStep();
                        Disable();
                    }
                }
                else if (isDraggingUp(touch)){
                    // check if step breathing in is correct
                    // if it is, increment InhalerLogic.Instance.CurrentStep
                    if (InhalerLogic.Instance.IsCurrentStepCorrect(breathingInStep)){
                        inhale.Play();
                        Debug.Log("Completed step" + breathingInStep);
                        InhalerLogic.Instance.NextStep();
                        completedGame = true;
                        // inhalerGameManager.OnGameEnd();
                        Disable();
                    }
                }
            }
        }
	}

    void CheckAndEnable(){
        if (InhalerLogic.Instance.CurrentStep == 3 || InhalerLogic.Instance.CurrentStep == breathingInStep){
            if (inhalerGameManager.ShowHint){
                renderer.enabled = true;
                breatheMessageObj.renderer.enabled = true;
            }
            else {
                renderer.enabled = false;
                breatheMessageObj.renderer.enabled = false;
            }
            collider.enabled = true;
        }
    }

    void Disable(){
        renderer.enabled = false;
        collider.enabled = false;
        breatheMessageObj.renderer.enabled = false;
    }

    void ResetTouch(){
        firstTouchOnObject = false;
    }

    bool isDraggingUp(Touch touch){
        if (touch.position.y - firstTouchYPos > 50){
            // Debug.Log("is dragging up");
            return true;
        }
        return false;
    }
    bool isDraggingDown(Touch touch){
        if (firstTouchYPos - touch.position.y > 50){
            // Debug.Log("is dragging down");
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
