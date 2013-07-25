using UnityEngine;
using System.Collections;

/*
    Advair Draggable Body (Advair Step 4).

    This listens for the user's touch on the body, and the inhaler "becomes" a smaller advair inhaler
    when dragged to the pet.

    How this happens is, when the user's touch starts on the inhaler, the original body is hidden.
    At the same time, a GUI texture is drawn under the user's touch position.

    If the inhaler is dragged to and released over the pet, it will disappear, and the
    step is completed.
    If it is released anywhere else, the original advair inhaler will reappear.
*/
public class InhalerBody : MonoBehaviour
{
    public Texture2D smallInhaler ;
    public Collider destinationCollider;

    bool inhalerDraggedToPet = false;
    bool showSmallInhaler = false;
    bool firstTouchOnObject = false;
    int dragToPetStep = 4;

    void Start(){
       collider.enabled = false;
       destinationCollider = GameObject.Find("PetSprite").collider;
    }
    void Update(){
        if (InhalerLogic.CurrentStep != dragToPetStep){
            return;
        }

        collider.enabled = true;
        // todo: perhaps we need to disable other colliders? leave for later

        if (Input.touchCount == 0) { // if not touching screen
            ResetTouch();
            ShowLargeInhaler();
        }
        else if(Input.touchCount > 0 && !inhalerDraggedToPet){
            Touch touch = Input.GetTouch(0);
            if (Input.GetMouseButtonDown(0) && isTouchingObject(touch)) {
                HideLargeInhaler();
                showSmallInhaler = true;
                firstTouchOnObject = true;
            }
            else if (Input.GetMouseButton(0) && firstTouchOnObject)
            {
                if (HasHitDestination(touch)){
                    if (InhalerLogic.IsCurrentStepCorrect(dragToPetStep)){
                        print("completed step " + dragToPetStep);
                        if (!InhalerLogic.IsDoneWithGame()){
                            InhalerLogic.NextStep();
                        }
                        HideLargeInhaler();
                        inhalerDraggedToPet = true;
                    }
                }
            }
        }
    }

    // Check if finger has been dragged into the specified collider.
    bool HasHitDestination(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;

        if (Physics.Raycast (ray, out hit)) {
            if(hit.collider == destinationCollider){
                return true;
            }
        }
        return false;
    }

    // Show the inhaler plus all its children.
    void ShowLargeInhaler(){
        renderer.enabled = true;
        Component[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
    }

    // Hide the inhaler plus all its children.
    void HideLargeInhaler(){
        renderer.enabled = false;
        Component[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = false;
        }
    }

    void ResetTouch(){
        showSmallInhaler = false;
        firstTouchOnObject = false;
    }

    // Only active at the right step.
    // When the player tries to drag the inhaler to the pet, hide the large inhaler,
    // and draw a small inhaler texture that follows the cursor (finger).
    void OnGUI(){
        if (!inhalerDraggedToPet && showSmallInhaler){
            if (Input.touchCount > 0){
                Touch touch = Input.GetTouch(0);
                GUI.DrawTexture(new Rect(touch.position.x - 50, Screen.height - touch.position.y - 50, 100, 100), smallInhaler);
            }
        }
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