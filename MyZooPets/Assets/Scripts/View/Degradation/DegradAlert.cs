using UnityEngine;
using System;

/// <summary>
/// Alert that triggers when pet hit by trigger
/// </summary>
public class DegradAlert : MonoBehaviour{
	void Start(){
		// begin listening for the callback for when the pet gets hit
		if(DegradationLogic.Instance != null) {
			DegradationLogic.Instance.OnPetHit += OnPetHit;
		}
	}

	void OnDestroy(){
		if(DegradationLogic.Instance != null) {
			DegradationLogic.Instance.OnPetHit -= OnPetHit;
		}
	}
	
	private void OnPetHit(object sender, EventArgs args){
		CameraManager.Instance.ShakeCamera();
	}
}
