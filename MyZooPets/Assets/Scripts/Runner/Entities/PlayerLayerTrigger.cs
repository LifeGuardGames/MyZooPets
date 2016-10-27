using UnityEngine;
using System.Collections.Generic;

public class PlayerLayerTrigger : MonoBehaviour {
	private List<Collider> mCurrentColliders = new List<Collider>();
	public List<Collider> CurrentColliders { get { return mCurrentColliders; } }

	void OnTriggerEnter(Collider inCollider) {
		if(inCollider.GetComponent<RunnerItem>() == null) {
			mCurrentColliders.Add(inCollider);
		}
		transform.parent.SendMessage("LayerTriggerCollisionEnter", inCollider);
	}

	void OnTriggerStay(Collider inCollider) {
		transform.parent.SendMessage("LayerTriggerCollisionStay", inCollider);
	}

	void OnTriggerExit(Collider inCollider) {
		if(inCollider.GetComponent<RunnerItem>() == null) {
			mCurrentColliders.Remove(inCollider);
		}
		transform.parent.SendMessage("LayerTriggerCollisionExit", inCollider);
	}
}
