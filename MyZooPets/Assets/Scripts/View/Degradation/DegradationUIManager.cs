using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationUIManager : Singleton<DegradationUIManager>{
    //When particle effects need to be turned on
    public static event EventHandler<EventArgs> OnActivateParticleEffects;

	// constants
	public const string TUT_TRIGGER = "TutorialTrigger";
	
    public GameObject cleanTriggerParticleDrop;
	public GameObject petHitLocation;	// Used for triggers to attach as end destination
	public bool bTesting; // turn this on so that triggers spawn no matter what...used for testing

    private List<GameObject> currentSpawnTriggers; //keep a reference of all the triggers spawned

	public bool IsTesting() {
		bool bTesting = Constants.GetConstant<bool>( "TestingDegrad" );
		return bTesting;
	}

    void Awake(){
        currentSpawnTriggers = new List<GameObject>();
    }

    void Start(){
        DegradationLogic.OnRefreshTriggers += PlaceTriggers;
        PlaceTriggers(this, EventArgs.Empty); //required for first initialization
    }

    void OnDestroy(){
        DegradationLogic.OnRefreshTriggers -= PlaceTriggers;
    }

    void OnApplicationPause(bool isPaused){
        //need to remove 
        if(isPaused)
            foreach(GameObject spawnedTrigger in currentSpawnTriggers)
                Destroy(spawnedTrigger);
    }
	
    //Use this to turn on all particle effects in triggers
    // now unused, I think
    public void ActivateParticleEffects(){
        if(OnActivateParticleEffects != null)
            OnActivateParticleEffects(this, EventArgs.Empty);
        else
            Debug.LogWarning("OnActivateParticleEffects is null");
    }

	//---------------------------------------------------
	// PlaceTutorialTrigger()
	// Places the lone tutorial trigger, which is the 0th
	// trigger.
	//---------------------------------------------------	
	public DegradTrigger PlaceTutorialTrigger() {
        List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;
        DegradData degradTrigger = (DegradData) degradTriggers.First();
		DegradTrigger trigger = PlaceTrigger(degradTrigger);
		trigger.gameObject.name = TUT_TRIGGER;
		
		return trigger;
	}

    //---------------------------------------------------
    // PlaceTriggers()
    //---------------------------------------------------       
    private void PlaceTriggers(object sender, EventArgs args) {
        // if the player has not yet played the trigger tutorial yet, we don't want to go spawning triggers
        bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManager_Bedroom.TUT_LAST);
        if (!bTriggers && !IsTesting())
            return;
        
        // loop through and place all defined triggers
        List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;
        for (int i = 0; i < degradTriggers.Count; i++)
            PlaceTrigger(degradTriggers[i]);        
    }
	
	//---------------------------------------------------
	// PlaceTrigger()
	// Note that this function assumes it is okay to be
	// spawning triggers.
	//---------------------------------------------------		
	private DegradTrigger PlaceTrigger(DegradData degradData) {		
        string triggerID = degradData.TriggerID;
        Vector3 position = degradData.GetPosition();
        ImmutableData_Trigger triggerData = DataLoader_Triggers.GetTrigger(triggerID);

        //Load trigger prefab
        GameObject triggerPrefab = (GameObject) Resources.Load(triggerData.PrefabName);

        //instantiate all the triggers save in DataManager
        GameObject trigger = (GameObject)Instantiate(triggerPrefab, position, Quaternion.identity);

        //keep a local reference
        currentSpawnTriggers.Add(trigger);
		
		// set the trigger's ID
        DegradTrigger scriptTrigger = trigger.GetComponent<DegradTrigger>();
		scriptTrigger.ID = triggerID;
		
		return scriptTrigger;
	}

}