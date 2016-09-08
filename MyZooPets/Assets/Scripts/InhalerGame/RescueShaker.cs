using UnityEngine;

/// <summary>
/// Inhaler shaker (Step 3)
/// Listens to drag gesture. Dragging is confined to a plan so it looks shaking the inhaler
/// </summary>
public class RescueShaker : InhalerPart {
	public GameObject shakerConstraintPlane; //the plane that restricts the drag motion
	public Animation scaleAnim;

	private Vector3 startDragPos;
	private bool doneWithShake = true; //disable shake after it's done
	private float elapsed;

	protected override void Awake() {
		base.Awake();
		gameStepID = 3;
		startDragPos = transform.position;
	}

	void OnDrag(DragGesture gesture) {
		// current gesture phase (Started/Updated/Ended)
		ContinuousGesturePhase phase = gesture.Phase;
		switch(phase) {
		case ContinuousGesturePhase.Ended:
			transform.position = startDragPos;
			if(doneWithShake)
				GetComponent<TBDragToMove>().DragPlaneCollider = null;
			break;
		case ContinuousGesturePhase.Updated:
			elapsed = gesture.ElapsedTime;
			if(!doneWithShake && elapsed >= 1f) { //Shake inhaler for 1 second
				if(!isGestureRecognized) {
					isGestureRecognized = true;
					scaleAnim.Play("InhalerObjectPulse");
					NextStep();
				}
			}
			break;
		}
	}

	void OnFingerHover(FingerHoverEvent e) {
		// check the hover event phase to check if we're entering or exiting the object
		if(e.Phase == FingerHoverPhase.Exit) {
			if(!doneWithShake) {
				//Cancel shaker if finger moves out of the Rescue game object
				transform.position = startDragPos;
				elapsed = 0;
			}
		}
	}

	void OnFingerStationary(FingerMotionEvent e) {
		if(!doneWithShake) {
			//Cancel shaker if finger stops moving and stays stationary
			transform.position = startDragPos;
			elapsed = 0;
		}
	}

	protected override void Disable() {
		transform.GetComponent<Collider>().enabled = false;
		shakerConstraintPlane.SetActive(false);
		doneWithShake = true;
	}

	protected override void Enable() {
		transform.GetComponent<Collider>().enabled = true;
		shakerConstraintPlane.SetActive(true);
		doneWithShake = false;
	}

	protected override void NextStep() {
		base.NextStep();

		// play sound here
		AudioManager.Instance.PlayClip("inhalerShake");

		Disable();
		transform.position = startDragPos;
	}
}