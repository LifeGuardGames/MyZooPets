using UnityEngine;
using System.Collections;

public class RescueShaker : MonoBehaviour {
    float shakeValue = 0f;
    float shakeIncrement = 0.5f;
    float shakeTarget = 10f;

    void Start(){
        renderer.enabled = false;
    }

    void Update(){
        if (InhalerLogic.CurrentStep != 2){
            return;
        }
        renderer.enabled = true;

        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (isTouchingObject(touch)){
                if (touch.phase == TouchPhase.Moved){
                    shakeValue += shakeIncrement;
                }
            }
        }
        if (shakeValue >= shakeTarget){
            // todo: play sound
            Debug.Log("shake target reached");

            if (InhalerLogic.IsCurrentStepCorrect(2)){
                InhalerLogic.IsDoneWithGame();
                renderer.enabled = false;
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