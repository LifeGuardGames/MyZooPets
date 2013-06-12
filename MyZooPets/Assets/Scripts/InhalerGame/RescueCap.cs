using UnityEngine;
using System.Collections;

public class RescueCap : MonoBehaviour {

    void Update()
    {
        if (InhalerLogic.CurrentStep != 1){
            return;
        }

        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            // if is clicked
            if (Input.GetMouseButtonDown(0)) {
                if (isTouchingObject(touch)){
                    if (InhalerLogic.IsCurrentStepCorrect(1)){
                        Destroy(gameObject);
                        if (!InhalerLogic.IsDoneWithGame()){
                            InhalerLogic.NextStep();
                        }
                    }
                }
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