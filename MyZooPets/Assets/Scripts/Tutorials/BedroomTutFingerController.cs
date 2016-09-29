using UnityEngine;
using System.Collections;

// One controller to rule them all
public class BedroomTutFingerController : MonoBehaviour {
	public enum FingerState {
		Press,
		Hold,
		DelayPress,
		FireCrystalDrag
	}

	public Animator fingerAnimator;

	public void PlayState(FingerState state) {
		switch(state) {
			case FingerState.Press:
				fingerAnimator.Play("BedroomTutFingerPress");
                break;
			case FingerState.DelayPress:
				fingerAnimator.Play("BedroomTutFingerNone");
				StartCoroutine(DelayPressHelper());
				break;
			case FingerState.Hold:
				fingerAnimator.Play("BedroomTutFingerHold");
				break;
			case FingerState.FireCrystalDrag:
				fingerAnimator.Play("BedroomTutFingerFireCrystal");
				break;
		}
	}

	private IEnumerator DelayPressHelper() {
		yield return new WaitForSeconds(1f);
		fingerAnimator.Play("BedroomTutFingerPress");
	}
}
