using UnityEngine;
using System;

/// <summary>
/// Handles inhale (swipe left) action
/// </summary>
public class Inhale : InhalerPart {
	public Animation InhalerBodyMoveAnimation;

	public static EventHandler<EventArgs> finish;
	protected override void Awake() {
		base.Awake();
		gameStepID = 7;
		//		floatyOptions.Add("text", ""); 
	}

	void OnSwipe(SwipeGesture gesture) {
		FingerGestures.SwipeDirection direction = gesture.Direction;
		if(direction == FingerGestures.SwipeDirection.Left) {
			if(!isGestureRecognized) {
				GesturePassAction();
			}
		}
	}

	void OnDrag(DragGesture gesture) {
		if(!isGestureRecognized) {
			Vector3 begin = new Vector3(0, 0, 0);
			Vector3 ended;
			if(gesture.Phase == ContinuousGesturePhase.Ended) {
				ended = gesture.Position;
				begin = gesture.StartPosition;
				if(begin.x > ended.x) {
					GesturePassAction();
                }
			}
		}
	}

	// Actual implementation of the result of two gestures
	public void GesturePassAction() {
		isGestureRecognized = true;

		//Disable hint when swipe gesture is registered. 
		GetComponent<HintController>().DisableHint(false);

		LgInhalerAnimationEventHandler.BreatheInEndEvent += BreatheInEndEventHandler;
		AudioManager.Instance.PlayClip("inhalerInhale");
		petAnimator.SetTrigger("BreatheIn");

		Debug.LogWarning("FLOATY SPAWN HERE");
		/*
		Hashtable option = new Hashtable();
		option.Add("parent", GameObject.Find("Canvas"));
		option.Add("text", Localization.Localize("INHALER_FLOATY_HOLD_BREATH"));
		option.Add("prefab", "FloatyTextInhalerGame");
		option.Add("textSize", 84f);
		option.Add("color", Color.white);

		FloatyUtil.SpawnFloatyText(option);
		*/

		if(finish != null) {
			finish(this, EventArgs.Empty);
		}
		InhalerBodyMoveAnimation.Play();

		// Custom override to show the last step
		InhalerGameUIManager.Instance.NextStepUI(8);
		GetComponent<HintController>().DisableHint(false);
	}

	protected override void Disable() {
		gameObject.SetActive(false);
	}

	protected override void Enable() {
		gameObject.SetActive(true);
	}

	protected override void NextStep() {
		base.NextStep();
		Destroy(gameObject);
	}

	private void BreatheInEndEventHandler(object sender, EventArgs args) {
		LgInhalerAnimationEventHandler.BreatheInEndEvent -= BreatheInEndEventHandler;
		AudioManager.Instance.FadeOutPlayNewBackground("inhalerCapstone", isLoop: false);
		petAnimator.SetTrigger("Backflip");

		// Hide the inhaler
		InhalerGameUIManager.Instance.HideInhaler();

		NextStep();
	}
}
