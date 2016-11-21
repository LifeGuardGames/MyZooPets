using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dragging the whole inhaler (step 5)
/// Listens to drag. gameobject will be snapback if it doesn't land on the target collider
/// </summary>
public class RescueDrag : InhalerPart {
	public List<GameObject> targetColliders = new List<GameObject>();
	public Vector3 targetDragPos; //Final position of the inhaler after drag
	public Animation scaleAnim;

	private Vector3 startDragPos; //Original position of the inhaler
	private bool doneWithDrag = true;

	protected void Awake() {
		gameStepID = 5;
		startDragPos = transform.position;
	}

	void OnDrag(DragGesture gesture) {
		// current gesture phase (Started/Updated/Ended)
		ContinuousGesturePhase phase = gesture.Phase;

		if(phase == ContinuousGesturePhase.Ended) { //Check where spacer has been dropped
			Ray ray = Camera.main.ScreenPointToRay(gesture.Position);
			RaycastHit hit;
			bool snapBack = true;
			int maskLayer = 1 << 9;

			//Snap to position if spacer is at target position or revert to start pos
			if(Physics.Raycast(ray, out hit, 100, maskLayer)) {
				foreach(GameObject targetCollider in targetColliders) {
					if(hit.collider.gameObject == targetCollider) {
						transform.position = targetDragPos;

						if(!doneWithDrag && !isGestureRecognized) {
							isGestureRecognized = true;
							scaleAnim.Play("InhalerObjectPulse");
							NextStep();
							snapBack = false;
						}
					}
				}
			}
			if(snapBack) {
				transform.position = startDragPos;
			}
		}
	}

	protected override void Disable() {
		foreach(GameObject targetCollider in targetColliders) {
			targetCollider.SetActive(false);
		}
	}

	protected override void Enable() {
		transform.GetComponent<Collider>().enabled = true;

		foreach(GameObject targetCollider in targetColliders) {
			targetCollider.SetActive(true);
		}

		doneWithDrag = false;
	}

	protected override void NextStep() {
		// play sound here
		AudioManager.Instance.PlayClip("inhalerToMouth");

		base.NextStep();
		transform.GetComponent<Collider>().enabled = false;

		foreach(GameObject targetCollider in targetColliders) {
			targetCollider.SetActive(false);
		}
		doneWithDrag = true;
	}
}