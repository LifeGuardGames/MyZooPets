using UnityEngine;
using System.Collections;

public class PlayerLayerTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider inCollider) {
        transform.parent.SendMessage("LayerTriggerCollisionEnter", inCollider);
    }

    void OnTriggerStay(Collider inCollider) {
        transform.parent.SendMessage("LayerTriggerCollisionStay", inCollider);
    }

    void OnTriggerExit(Collider inCollider) {
        transform.parent.SendMessage("LayerTriggerCollisionExit", inCollider);
    }
}
