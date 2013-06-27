using UnityEngine;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
    private DegradationLogic degradationLogic;

	// Use this for initialization
	void Start () {
        degradationLogic = GameObject.Find("GameManager").GetComponent<DegradationLogic>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.touchCount > 0){
            if(IsTouchingObject(Input.GetTouch(0))){
                //when trigger is touched remove from DataManager and destroy GameObject
                degradationLogic.ClearDegradationTrigger(id);
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
