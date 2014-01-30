using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationUIManager : Singleton<DegradationUIManager>{
	// constants
	public const string TUT_TRIGGER = "TutorialTrigger";
	
    //When particle effects need to be turned on
    public static event EventHandler<EventArgs> OnActivateParticleEffects;
    public GameObject cleanTriggerParticleDrop;
	
	public GameObject petHitLocation;	// Used for triggers to attach as end destination
	
	// turn this on so that triggers spawn no matter what...used for testing
	public bool bTesting;
	public bool IsTesting() {
		bool bTesting = Constants.GetConstant<bool>( "TestingDegrad" );
		return bTesting;
	}

    void Start(){
        DegradationLogic.OnRefreshTriggers += PlaceTriggers;
        PlaceTriggers(this, EventArgs.Empty); //required for first initialization
    }

    void OnDestroy(){
        DegradationLogic.OnRefreshTriggers -= PlaceTriggers;
    }
	
	//---------------------------------------------------
	// PlaceTriggers()
	//---------------------------------------------------		
	private void PlaceTriggers(object sender, EventArgs args) {
		// if the player has not yet played the trigger tutorial yet, we don't want to go spawning triggers
		bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TutorialManager_Bedroom.TUT_LAST );
		if ( !bTriggers && !IsTesting() )
			return;
		
		// loop through and place all defined triggers
		List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;
        for ( int i = 0; i < degradTriggers.Count; i++ )
			PlaceTrigger( i );		
	}
	
	//---------------------------------------------------
	// PlaceTutorialTrigger()
	// Places the lone tutorial trigger, which is the 0th
	// trigger.
	//---------------------------------------------------	
	public DegradTrigger PlaceTutorialTrigger() {
		DegradTrigger trigger = PlaceTrigger( 0 );
		trigger.gameObject.name = TUT_TRIGGER;
		
		return trigger;
	}
	
	//---------------------------------------------------
	// PlaceTrigger()
	// Note that this function assumes it is okay to be
	// spawning triggers.
	//---------------------------------------------------		
	private DegradTrigger PlaceTrigger(int nIndex) {		
        List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;

        string triggerID = degradTriggers[nIndex].TriggerID;
        Vector3 position = degradTriggers[nIndex].GetPosition();
        ImmutableData_Trigger triggerData = DataLoader_Triggers.GetTrigger(triggerID);

        //Load trigger prefab
        GameObject triggerPrefab = (GameObject) Resources.Load(triggerData.PrefabName);

        //instantiate all the triggers save in DataManager
        GameObject trigger = (GameObject)Instantiate(triggerPrefab, position, Quaternion.identity);
		
		// set the trigger's ID
        DegradTrigger scriptTrigger = trigger.GetComponent<DegradTrigger>();
		scriptTrigger.ID = triggerID;
		
		return scriptTrigger;
	}

    //Use this to turn on all particle effects in triggers
	// now unused, I think
    public void ActivateParticleEffects(){
        if(OnActivateParticleEffects != null)
            OnActivateParticleEffects(this, EventArgs.Empty);
        else
            Debug.LogWarning("OnActivateParticleEffects is null");
    }
}