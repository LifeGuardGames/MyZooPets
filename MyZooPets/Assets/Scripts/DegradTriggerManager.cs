using UnityEngine;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
	// Use this for initialization

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.touchCount > 0){
            if(IsTouchingObject(Input.GetTouch(0))){
                DegradationLogic.ClearDegradationTrigger(id);
                Destroy(this.gameObject);
            }
        }	
	}

    private bool IsTouchingObject(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        bool retVal = false;

        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.gameObject == this.gameObject){
                retVal = true;
            }
        }
        return retVal;
    }
}
