using UnityEngine;
using System.Collections;

public class InhalerBody : MonoBehaviour
{
    public Texture2D smallInhaler ;

    bool completelyOpened = false;
    bool showSmallInhaler = false;
    bool firstTouchOnObject = false;

    void Start(){
       collider.enabled = false;
    }
    void Update(){
        // if (InhalerLogic.CurrentStep != 4){
            // todo: set to 4 later
        if (InhalerLogic.CurrentStep != 3){
            return;
        }
        else {
            collider.enabled = true;
            // todo: perhaps we need to disable other colliders? leave for later

            if (Input.touchCount == 0) { // if not touching screen
                ShowLargeInhaler();
                // todo:
                // SnapBack();
                ResetTouch();
            }
            else if(Input.touchCount > 0 && !completelyOpened){
                Touch touch = Input.GetTouch(0);
                if (Input.GetMouseButtonDown(0) && isTouchingObject(touch)) {
                    HideLargeInhaler();
                    showSmallInhaler = true;
                    firstTouchOnObject = true;
                }
                else if (Input.GetMouseButton(0) && firstTouchOnObject)
                {
                    // todo: if cursor is in target area, then do something
                }
            }

        }
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
        if (showSmallInhaler){
            Touch touch = Input.GetTouch(0);
            GUI.DrawTexture(new Rect(touch.position.x - 50, Screen.height - touch.position.y - 50, 100, 100), smallInhaler);
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