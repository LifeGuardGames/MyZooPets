using UnityEngine;
using System.Collections;

// ================================================================================================
/*
    PetMovement:

    What it does:
        When the user taps a spot on the floor, the pet moves to that spot.

    To use PetMovement:
        1)Attach this script to the floor collider GameObject.
        1)Attach these other scripts:
            1) TapGesture (from TouchScript.Gestures)
            2) TapItem
        2) Make sure Init() is called somewhere else.

    What this does:
        When a proper tap (configurable in TapGesture, in the inspector) is performed on
        the floor GameObject, MovePet() is called.
        MovePet() checks the tap's screen position, and moves the pet to that corresponding
        location.
*/
// ================================================================================================

public class PetMovement : MonoBehaviour {

    public GameObject petSprite;
    private Vector3 destinationPoint;
    private TapItem tapItem;

	public void Init () {
        petSprite = GameObject.Find("SpritePet");
        destinationPoint = petSprite.transform.position;

        tapItem = GetComponent<TapItem>();
        tapItem.OnTap += MovePet;
	}

    void MovePet(){
        // if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
        if (!ClickManager.CanRespondToTap()) return;
        // temporary fix for making sure pet doesn't move when GUI elements are clicked
        if(tapItem.lastTapPosition.y < 110) return;

        Ray myRay = Camera.main.ScreenPointToRay(tapItem.lastTapPosition);
        RaycastHit hit;
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == collider){
                destinationPoint = hit.point;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (petSprite != null){
            if (ClickManager.CanRespondToTap()){
                petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,destinationPoint,5f * Time.deltaTime);
            }
        }
	}
}
