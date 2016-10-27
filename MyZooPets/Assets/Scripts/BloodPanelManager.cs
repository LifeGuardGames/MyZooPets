using UnityEngine;

public class BloodPanelManager : Singleton<BloodPanelManager> {
	public Animator bloodAnimator;

	// One shot of blood
	public void PlayBlood() {
		bloodAnimator.SetTrigger("Blood");
	}
}
