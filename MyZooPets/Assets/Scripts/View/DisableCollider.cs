using UnityEngine;
using System.Collections;

/*
    Used mainly for shelf and trophies right now. The shelf's collider blocks the trophy colliders, so we
    are disabling the shelf's collider here. This is most likely a temporary solution.
*/

public class DisableCollider : MonoBehaviour {

    void Update(){
        if (ClickManager.CanRespondToTap()){
            collider.enabled = true;
        }
        else {
            collider.enabled = false;
        }
    }
}
