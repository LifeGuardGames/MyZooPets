using UnityEngine;
using System.Collections;

public class RescueSwitch : MonoBehaviour {

    void Update()
    {
        if (InhalerLogic.CurrentStep != 5){
            return;
        }

        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            // if is clicked
            if (Input.GetMouseButtonDown(0)) {
                if (isTouchingObject(touch)){
                    if (InhalerLogic.IsCurrentStepCorrect(5)){
                        if (!InhalerLogic.IsDoneWithGame()){
                            InhalerLogic.NextStep();
                        }
                        InhalerClickAnimation();
                    }
                }
            }
        }
    }

    void InhalerClickAnimation(){
        Hashtable optional = new Hashtable();
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        LeanTween.move(gameObject, new Vector3(x,y - 2f,z), 0.5f, optional);
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