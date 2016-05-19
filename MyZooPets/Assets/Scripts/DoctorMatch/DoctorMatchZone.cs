using UnityEngine;
using UnityEngine.UI;

public class DoctorMatchZone : MonoBehaviour {
	public Button button;
	public DoctorMatchManager.DoctorMatchButtonTypes buttonType;
	public ParticleSystem particle;

	private float lockTime = 0.05f;
	private bool isLocked = false;

	public void ToggleButtonInteractable(bool isInteractive) {
		button.interactable = isInteractive;
	}

	public void OnZoneClicked(){
		if(!isLocked) {
			isLocked = true;	// Lock to prevent accidental double tapping
            DoctorMatchManager.Instance.OnZoneClicked(buttonType);
			particle.Play();
			Invoke("TempUnlock", lockTime);
		}
	}

	public void TempUnlock() {
		isLocked = false;
	}
}
