using UnityEngine;
using System;
using System.Collections;

public class TutorialLogic : Singleton<TutorialLogic> {

    //======================Events======================
    //called when any one of the tutorials is completed. UI needs to be reset 
    public static event EventHandler<EventArgs> OnTutorialUpdated;
    //==================================================
    public bool FirstTimeCalendar{
        get{
            return DataManager.Instance.Tutorial.FirstTimeCalendar;
        }
        set{
            DataManager.Instance.Tutorial.FirstTimeCalendar = value;
            if(OnTutorialUpdated != null){
                OnTutorialUpdated(this, EventArgs.Empty);
            }else{
                Debug.LogError("OnTutorialUpdated is null");
            }
        }
    }

    public bool FirstTimeRealInhaler{
        get{
            return DataManager.Instance.Tutorial.FirstTimeRealInhaler;
        }
        set{
            DataManager.Instance.Tutorial.FirstTimeRealInhaler = value;
            if(OnTutorialUpdated != null){
                OnTutorialUpdated(this, EventArgs.Empty);
            }else{
                Debug.LogError("OnTutorialUpdated is null");
            }
        }
    }

    public bool FirstTimeDegradTrigger{
        get{
            return DataManager.Instance.Tutorial.FirstTimeDegradTrigger;
        }
        set{
            DataManager.Instance.Tutorial.FirstTimeDegradTrigger = value;
            if(OnTutorialUpdated != null){
                OnTutorialUpdated(this, EventArgs.Empty);
            }else{
                Debug.LogError("OnTutorialUpdated is null");
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
