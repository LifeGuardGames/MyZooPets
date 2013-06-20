using UnityEngine;
using System.Collections;

public class RescueBody : MonoBehaviour
{
    public Texture2D smallInhaler ;
    public Collider destinationCollider;
    public GameObject miniature;

    bool inhalerDraggedToPet = false;
    bool showSmallInhaler = false;
    bool firstTouchOnObject = false;
    bool hitDestination = false;
    int dragToPetStep = 4;
    // bool foundMiniature = false;

    // only moving in the y-axis
    bool goingUp = true;
    Vector3 shakeLowerBound;
    Vector3 shakeUpperBound;
    float shakeYDisplacement = 1.0f;

    void Start(){
       collider.enabled = false;
       destinationCollider = GameObject.Find("PetSprite").collider;
       miniature.SetActive(false); // shouldn't be null

       shakeLowerBound = transform.position;
       float yUpper = 2.0f;
       shakeUpperBound = new Vector3(transform.position.x, transform.position.y + yUpper, transform.position.z);

    }
    void Update(){
        if (hitDestination){
            HideLargeInhaler();
        }

        // if (!foundMiniature){
        //     miniature = GameObject.Find("SmallRescueBody");
        //     if (miniature != null){
        //         foundMiniature = true;
        //         miniature.SetActive(false);
        //     }
        // }

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
                        hitDestination = true;
                        inhalerDraggedToPet = true;
                    }
                }
            }
        }
    }

    bool HasHitDestination(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;

        if (Physics.Raycast (ray, out hit)) {
            if(hit.collider == destinationCollider){
                ShowSmallerInhaler();
                return true;
            }
        }
        return false;
    }

    void ShowSmallerInhaler(){
        miniature.SetActive(true);
    }

    void ShowLargeInhaler(){
        renderer.enabled = true;
        Component[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
    }

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

    public void Shake(){
        if (goingUp){
            // transform.position += new Vector3(0, shakeYDisplacement * Time.deltaTime, 0);
            transform.position += new Vector3(0, shakeYDisplacement, 0);
            if (transform.position.y > shakeUpperBound.y){
                transform.position = shakeUpperBound;
                goingUp = false;
            }
        }
        else {
            // transform.position -= new Vector3(0, shakeYDisplacement * Time.deltaTime, 0);
            transform.position -= new Vector3(0, shakeYDisplacement, 0);
            if (transform.position.y < shakeLowerBound.y){
                transform.position = shakeLowerBound;
                goingUp = true;
            }

        }
    }
}