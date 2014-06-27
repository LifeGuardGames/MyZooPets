using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Enable entrance on gate destroyed.
/// Attached this script on any mini game entrance that should be activated after
/// the gate in room partition # has been destroyed.
/// </summary>
public class EnableEntranceOnGateDestroyed : MonoBehaviour {
	public int roomPartition;

	// Use this for initialization
	void Start () {
		CheckToActivateEntrance();
		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;
	}

	void OnDestroy(){
		GatingManager.OnDestroyedGate -= OnDestroyedGateHandler;
	}

	private void OnDestroyedGateHandler(object sender, EventArgs args){
		CheckToActivateEntrance();
	}

	private void CheckToActivateEntrance(){
		bool isEntranceActive = !GatingManager.Instance.HasActiveGate(roomPartition);
		if(isEntranceActive)
			this.gameObject.SetActive(true);
		else
			this.gameObject.SetActive(false);
	}
}
