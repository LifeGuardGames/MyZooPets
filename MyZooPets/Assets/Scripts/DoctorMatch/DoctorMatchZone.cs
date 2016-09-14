using UnityEngine;
using UnityEngine.UI;

public class DoctorMatchZone : MonoBehaviour{
	public Button button;
	public EmergencyMicro parent;
	//Used to determine if we are in DoctorMatch or MicroMix
	public DoctorMatchGameManager.DoctorMatchButtonTypes buttonType;
	public ParticleSystem particle;

	private float lockTime = 0.05f;
	private bool isLocked = false;

	public void ToggleButtonInteractable(bool isInteractive){
		button.interactable = isInteractive;
	}

	public void OnZoneClicked(){
		Debug.Log(isLocked);
		if(!isLocked){
			TempLock(lockTime);// Lock to prevent accidental double tapping
			if(parent == null){
				DoctorMatchGameManager.Instance.OnZoneClicked(buttonType);
			}
			else{
				parent.OnZoneClicked(buttonType);
			}
			particle.Play();
		}
	}

	public void TempLock(float timeToLock){
		isLocked = true;	
		Invoke("TempUnlock", timeToLock);
		ToggleButtonInteractable(false);
	}

	public void TempUnlock(){
		isLocked = false;
		ToggleButtonInteractable(true);
	}
}
