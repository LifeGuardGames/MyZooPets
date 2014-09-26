using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Enable entrance on gate destroyed.
/// Attached this script on any mini game entrance that should be activated after
/// certain events.
/// Currently it will check if minipet has finished in room + gate unlocked
/// </summary>
public class EnableEntranceOnEvent : MonoBehaviour {
	public int roomPartition;
	
	// Use this for initialization
	void Start () {
		CheckToActivateEntrance();
//		GatingManager.OnDestroyedGate += EnableEntrance;
		MiniPetHUDUIManager.Instance.OnManagerOpen += EnableEntrance;	// When manager is closed
	}
	
	void OnDestroy(){
//		GatingManager.OnDestroyedGate -= EnableEntrance;
	}
	
	private void EnableEntrance(object sender, UIManagerEventArgs args){
		if(args.Opening == false){
			CheckToActivateEntrance();
		}
	}
	
	private void CheckToActivateEntrance(){
		// NOTE: Checking after loading again will just check if gate unlocked!
		bool isEntranceActive = !GatingManager.Instance.HasActiveGate(roomPartition);
		if(isEntranceActive)
			this.gameObject.SetActive(true);
		else
			this.gameObject.SetActive(false);
	}
}
