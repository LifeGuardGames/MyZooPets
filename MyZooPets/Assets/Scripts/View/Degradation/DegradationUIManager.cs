using UnityEngine;
using System;
using System.Linq;
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

	void Awake(){
		currentSpawnTriggers = new List<GameObject>();
	}

	void Start() {
		DegradationLogic.OnRefreshTriggers += PlaceTriggers;
		PlaceTriggers(this, EventArgs.Empty); //required for first initialization
	}

	void OnDestroy(){
		DegradationLogic.OnRefreshTriggers -= PlaceTriggers;
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
	public DegradTrigger PlaceTutorialTrigger(){
		List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;

		DegradData degradTrigger = degradTriggers.First();
		DegradTrigger trigger = PlaceTrigger(degradTrigger);
		trigger.gameObject.name = TUT_TRIGGER;

		return trigger;
	}

	private void CleanupExistingTriggers(){
		foreach(GameObject spawnedTrigger in currentSpawnTriggers){
			//check null first because some of the triggers might be cleaned up
			//by the user already
			if(spawnedTrigger){
				spawnedTrigger.SetActive(false);
				Destroy(spawnedTrigger);
			}
		}
		currentSpawnTriggers = new List<GameObject>();
	}
  
	private void PlaceTriggers(object sender, EventArgs args){
		//to make sure no left over triggers exists. clean up first
		CleanupExistingTriggers();
        
		// loop through and place all defined triggers
		List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;
		if(degradTriggers == null || degradTriggers.Count == 0) {
			return;
		}
		else {
			for(int i = 0; i < degradTriggers.Count; i++) {
				PlaceTrigger(degradTriggers[i]);
			}
		}
	}

	// Note that this function assumes it is okay to be spawning triggers.
	private DegradTrigger PlaceTrigger(DegradData degradData){		
		string triggerID = degradData.TriggerID;
		int partition = degradData.Partition;
		Vector3 position = degradData.Position;

		ImmutableDataTrigger triggerData = DataLoaderTriggers.GetTriggerData(triggerID);

		//instantiate all the triggers saved in DataManager
		Transform parent = PartitionManager.Instance.GetInteractableParent(partition);
		GameObject triggerPrefab = (GameObject)Resources.Load(triggerData.PrefabName);
		GameObject trigger = GameObjectUtils.AddChild(parent.gameObject, triggerPrefab);
		trigger.transform.localPosition = position;

		//keep a local reference
		currentSpawnTriggers.Add(trigger);
		
		// set the trigger's ID
		DegradTrigger scriptTrigger = trigger.GetComponent<DegradTrigger>();
		scriptTrigger.ID = triggerID;
		
		return scriptTrigger;
	}
}