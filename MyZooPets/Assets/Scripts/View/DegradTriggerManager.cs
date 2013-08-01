using UnityEngine;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
    private DegradationLogic degradationLogic;

	// Use this for initialization
	void Start () {
        degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
        GetComponent<TapItem>().OnTap += OnTap;
        if (DataManager.Instance.Tutorial.FirstTimeDegradTrigger){
            Tutorial tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
            GetComponent<TapItem>().OnTap += tutorial.DegradTriggerClicked;
        }
	}

    void OnTap(){
        //when trigger is touched remove from DataManager and destroy GameObject
        if (ClickManager.CanRespondToTap()){
            degradationLogic.ClearDegradationTrigger(id);
            Destroy(this.gameObject);
        }
    }
}
