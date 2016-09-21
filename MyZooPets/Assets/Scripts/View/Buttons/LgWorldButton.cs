using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Base class for a world button (ie. Inhaler button, anything in scene that is clickable, NOT UI)
/// Makes sure that the button can process a click.
/// </summary>
public abstract class LgWorldButton : MonoBehaviour {
	public EventHandler<EventArgs> OnProcessed;     // Launch event when this button is processed

	public List<UIModeTypes> modeTypes = new List<UIModeTypes>();
	public List<UIModeTypes> ModeTypes {
		get { return ModeTypes; }
	}

	public string buttonSound;
	public bool isCheckingClickManager;
	public bool isSprite;

	/// <summary>
	/// Raises the click event.
	/// 2D sprite buttons will receive this event, which will click the button.  At the moment 3D objects
	/// also happen to receive this event, but it's possible they won't in the future, so this is for 2D only.
	/// </summary>
	void OnClick() {
		if(enabled && isSprite) {
			ButtonClicked();
		}
	}

	/// <summary>
	/// Raises the press event.
	/// 2D sprite buttons - Play the sound when the button is pressed
	/// </summary>
	void OnPress(bool isPressed) {
		if(isPressed) {
			CheckSoundToPlay();
		}
	}

	/// <summary>
	/// Raises the tap event. 
	/// 3D gameObjects will receive this event.
	/// </summary>
	void OnTap(TapGesture gesture) {
		ButtonClicked();
		CheckSoundToPlay();
	}

	/// <summary>
	/// Raises the finger stationary event.
	/// 3D objects - Play the sound when the object is pressed down
	/// </summary>
	void OnFingerStationary(FingerMotionEvent e) {
		if(e.Phase == FingerMotionPhase.Started) {
			CheckSoundToPlay();
		}
	}

	/// <summary>
	/// When button is actually clicked.
	/// </summary>
	public void ButtonClicked() {
		// if the button needs to check the click manager before proceding, do so and return if necessary
		if(isCheckingClickManager && !ClickManager.Instance.CanRespondToTap(gameObject)) {
			return;
		}

		// special case hack here...if we are in a tutorial, regardless of if we are supposed to check the click manager, check it
		if(isCheckingClickManager == false && TutorialManager.Instance && !TutorialManager.Instance.CanProcess(gameObject)) {
			return;
		}

		// let anything listening know that this button has been processed
		if(OnProcessed != null) {
			OnProcessed(this, EventArgs.Empty);
		}

		// process the click
		ProcessClick();
	}

	/// <summary>
	/// Processes the click.
	/// Children should implement this. This function will only be called if the
	/// button is allowed to process the click (i.e., UI is not locked, etc).
	/// </summary>
	protected abstract void ProcessClick();
	
	/// <summary>
	/// Checks the sound to play.
	/// Play click sound or negative sound
	/// </summary>
	private void CheckSoundToPlay() {
		if(isCheckingClickManager && !ClickManager.Instance.CanRespondToTap(gameObject)) {
			if(isSprite) {
				// Play the bad sound
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			return;
		}

		if(!isCheckingClickManager && TutorialManager.Instance && !TutorialManager.Instance.CanProcess(gameObject)) {
			if(isSprite) {
				// Play the bad sound
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			return;
		}

		// Play the good sound
		PlayProcessSound();
	}

	private void PlayProcessSound() {
		if(!string.IsNullOrEmpty(buttonSound)) {
			AudioManager.Instance.PlayClip(buttonSound);
		}
	}
}
