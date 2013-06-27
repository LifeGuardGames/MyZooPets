using UnityEngine;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
	// Use this for initialization
	void Start () {
        GetComponent<TapItem>().OnTap += OnTap;
	}

    void OnTap(){
        //when trigger is touched remove from DataManager and destroy GameObject
        DegradationLogic.ClearDegradationTrigger(id);
        Destroy(this.gameObject);
    }
}
