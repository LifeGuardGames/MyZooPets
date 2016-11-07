using UnityEngine;

/// <summary>
/// Step 6
/// Listens to press gesture
/// </summary>
public class RescuePrescription : InhalerPart {
	protected void Awake() {
		gameStepID = 6;
	}

	void OnTap(TapGesture gesture) {
		LeanTween.moveLocalY(gameObject, 3.82f, 0.3f).setEase(LeanTweenType.easeInOutQuad);
		NextStep();
	}

	#if UNITY_EDITOR
	void Update() {
		if(Input.GetKeyDown(KeyCode.P)) {
			if(InhalerGameManager.Instance.IsCurrentStepCorrect(gameStepID)) {
				OnTap(null);
            }
		}
	}
	#endif

	protected override void Disable() {
		transform.GetComponent<Collider>().enabled = false;
	}

	protected override void Enable() {
		transform.GetComponent<Collider>().enabled = true;
	}

	protected override void NextStep() {
		AudioManager.Instance.PlayClip("inhalerSqueeze");

		base.NextStep();
		Disable();
	}
}