using UnityEngine;
using System;
using System.Collections;

public class TutorialLogic : Singleton<TutorialLogic> {
    //======================Events======================
    //called when any one of the tutorials is completed. UI needs to be reset 
    public static event EventHandler<EventArgs> OnTutorialUpdated;
    //==================================================
    public bool FirstTimeDegradTrigger{
        get{
            return DataManager.Instance.GameData.Tutorial.FirstTimeDegradTrigger;
        }
        set{
            DataManager.Instance.GameData.Tutorial.FirstTimeDegradTrigger = value;
            UpdateCallBack();
        }
    }

    private void UpdateCallBack(){
        if(D.Assert(OnTutorialUpdated != null, "OnTutorialUpdated has no listeners"))
            OnTutorialUpdated(null, EventArgs.Empty);
    }
}
