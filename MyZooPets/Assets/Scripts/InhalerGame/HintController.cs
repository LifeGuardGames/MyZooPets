using UnityEngine;

//This generic class controls the hint arrows and message for inhaler parts
public class HintController : MonoBehaviour {
	//	static bool isPlayingAnimation = false;
	static int currentStepAux = 1;

	public bool startHidden = true;		// Hide hint when game loads
	public int showOnStep = 0;			// Set this to the step that the hint arrow should be shown on.
	public Animator fingerAnimator;		// Animator that contains the hint animation

	public string clipToPlay;			// The animation that you want to play
	public GameObject outlineTexture;	// Texture of outline if applicable
	public GameObject textObject;		// GUI label of the text

	void Awake() {
		if(startHidden) {
			if(fingerAnimator != null) {
				fingerAnimator.gameObject.SetActive(false);
			}
			if(outlineTexture != null) {
				outlineTexture.SetActive(false);
			}
			if(textObject != null) {
				textObject.SetActive(false);
			}
		}
		InhalerGameUIManager.HintEvent += CheckAndEnableHint;
	}

	void OnDestroy() {
		InhalerGameUIManager.HintEvent -= CheckAndEnableHint;
	}

	//Event Listener. Check if hint for the next step is necessary and disable hint for current step 
	private void CheckAndEnableHint(object sender, InhalerHintEventArgs args) {
		if(!args.IsDisplayingHint && !InhalerGameUIManager.Instance.tutOn) {
			DisableHint(false);
		}
		else {
			bool isSkipAnimation = true;
			// Reset the animation if there current animation is not valid
			if(currentStepAux != InhalerGameManager.Instance.CurrentStep) {
				isSkipAnimation = false;
				currentStepAux = InhalerGameManager.Instance.CurrentStep;
			}

			// NOTE: Since the animator is shared thru all objects, we want to take care not to overwrite with separate scripts
			// The outline texture and the text object can be treated normally
			DisableHint(isSkipAnimation);

			if(InhalerGameUIManager.Instance.ShowHint &&
				showOnStep == InhalerGameManager.Instance.CurrentStep) {
				EnableHint();
			}
		}
	}

	// Turn off hint
	// NOTE: Since the animator is shared thru all objects, we want to take care not to overwrite with separate scripts
	// The outline texture and the text object can be treated normally
	public void DisableHint(bool skipAnimation) {
		if(!skipAnimation) {
			if(fingerAnimator != null) {
				fingerAnimator.gameObject.SetActive(false);
			}
		}
		if(outlineTexture != null) {
			outlineTexture.SetActive(false);
		}
		if(textObject != null) {
			textObject.SetActive(false);
		}
	}

	//Turn on hint
	private void EnableHint() {
		if(fingerAnimator != null) {
			fingerAnimator.gameObject.SetActive(true);
			fingerAnimator.Play(clipToPlay);
		}
		if(outlineTexture != null) {
			outlineTexture.SetActive(true);
		}
		if(textObject != null) {
			textObject.SetActive(true);
		}
	}
}
