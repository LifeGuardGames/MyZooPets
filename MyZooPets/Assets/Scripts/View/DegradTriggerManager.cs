using UnityEngine;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
    private DegradationLogic degradationLogic;

	// Use this for initialization
	void Start () {
        degradationLogic = GameObject.Find("GameManager").GetComponent<DegradationLogic>();
        GetComponent<TapItem>().OnTap += OnTap;
	}

    void OnTap(){
        //when trigger is touched remove from DataManager and destroy GameObject
            degradationLogic.ClearDegradationTrigger(id);
        Destroy(this.gameObject);
    }
}
