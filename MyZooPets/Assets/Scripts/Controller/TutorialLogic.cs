﻿using UnityEngine;
using System;
using System.Collections;

public class TutorialLogic : Singleton<TutorialLogic> {

    //======================Events======================
    //called when any one of the tutorials is completed. UI needs to be reset 
    public static event EventHandler<EventArgs> OnTutorialUpdated;
    //==================================================
    public bool FirstTimeCalendar{
        get{
            return DataManager.Instance.GameData.Tutorial.FirstTimeCalendar;
        }
        set{
            DataManager.Instance.GameData.Tutorial.FirstTimeCalendar = value;
            UpdateCallBack();
        }
    }

    public bool FirstTimeRealInhaler{
        get{
            return DataManager.Instance.GameData.Tutorial.FirstTimeRealInhaler;
        }
        set{
            DataManager.Instance.GameData.Tutorial.FirstTimeRealInhaler = value;
            UpdateCallBack();
        }
    }

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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
