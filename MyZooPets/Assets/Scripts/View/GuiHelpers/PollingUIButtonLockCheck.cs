using UnityEngine;
using System.Collections;

/// <summary>
/// Polling button lock check.
/// For NGUI UIButtons. Polls a bunch of checks before enabling the (NGUI)UIButtonMessage script on an object.
/// </summary>

public class PollingUIButtonLockCheck : MonoBehaviour {
	
	public bool checkClickManager = false;
	public bool isClickManagerModeLocked = false;
	public bool isClickManagerClickLocked = false;
	
	UIButtonMessage messageScript;
	
	void Start () {
		messageScript = gameObject.GetComponent<UIButtonMessage>();
	
		messageScript.enabled = false;
	}

	void Update () {
		if(checkClickManager){
			
			Debug.Log(ClickManager.isModeLocked + "   " + ClickManager.isClickLocked);
			if(ClickManager.isModeLocked == isClickManagerModeLocked && ClickManager.isClickLocked == isClickManagerClickLocked){
				messageScript.enabled = true;
				
				// Kill thyself
				this.enabled = false;
			}
		}
		else{
			messageScript.enabled = true;
			
			// Nothing to check, suicide
			this.enabled = false;
		}
	}
}
