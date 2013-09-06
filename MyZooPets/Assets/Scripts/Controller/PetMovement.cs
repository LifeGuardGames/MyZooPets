using UnityEngine;
using System.Collections;

// ================================================================================================
/*
    PetMovement:

    What it does:
        1. When the user taps a spot on the floor, the pet moves to that spot.
        2. If turned on, pet will walk around in the room randomly.
        3. when camera moves to another room, pet will move into that room.

    To use PetMovement:
        1)Attach this script to the floor collider GameObject.
        1)Attach these other scripts:
            1) TapGesture (from TouchScript.Gestures)
            2) TapItem

    What this does:
        When a proper tap (configurable in TapGesture, in the inspector) is performed on
        the floor GameObject, MovePet() is called.
        MovePet() checks the tap's screen position, and moves the pet to that corresponding
        location.
*/
// ================================================================================================

public class PetMovement : Singleton<PetMovement> {
    public Camera mainCamera;
    public GameObject shadowObject;     // The shadow of the pet
    public GameObject runWay; //Where the pet is allowed to move
    public GameObject petSprite;

    private tk2dSpriteAnimator anim; //2D sprite animator
    private Vector3 destinationPoint; //destination that the pet is going to move to
    private TapItem tapItem; //Tap gesture
	private bool moving; //Is Pet moving now or not
	private float moveToX;
	private float moveToZ;

    void Awake(){
        D.Assert(mainCamera != null, "Camera missing in " + this);
        D.Assert(petSprite != null, "PetSprite missing in " + this);
        // tapItem = GetComponent<TapItem>();
        anim = petSprite.GetComponent<tk2dSpriteAnimator>();
    }

    void Start(){
       destinationPoint = petSprite.transform.position;
       // tapItem.OnTap += MovePet;
    }

    // Update is called once per frame
    void Update () {
        if (moving && petSprite != null){
            if (ClickManager.Instance.CanRespondToTap()){ //move the pet location if allowed
                petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,
                    destinationPoint,8f * Time.deltaTime);
            }

            //when the sprite reaches destination. stop transform and animation
            if(petSprite.transform.position == destinationPoint){
                moving = false;
                anim.Stop();
                anim.Play("HappyIdle");
            }
        }
    }

    //Listen to OnTap Event from FingerGesture
    void OnTap(TapGesture gesture) { 
        // if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
        if(!ClickManager.Instance.CanRespondToTap()) return;

        MovePet(Camera.main.ScreenPointToRay(gesture.Position));    
    }

	public void MovePetWithCamera(){
		MovePet(mainCamera.ScreenPointToRay(new Vector3(Screen.width/3, 80, 0)));
	}

    //What to do when animation is finished playing. 
    private void OnAnimationFinished(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip){
        if(moving){
            anim.Play("HappyWalk");
        }else{
            anim.Play("HappyIdle");
        }
    }

    //Check if the touch is in walkable area then move/animate pet
    private void MovePet(Ray myRay){
        RaycastHit hit;
        // Debug.DrawRay(myRay.origin, myRay.direction * 50, Color.green, 50f);
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == runWay.collider){
                destinationPoint = hit.point;

                if(!anim.IsPlaying("HappyWalk")){
                    anim.Play("HappyWalk");
                    anim.AnimationCompleted = OnAnimationFinished;
                }
                moving = true;
            }
        }
        ChangePetFacingDirection();
    }

    //Decides when to flip sprite by comparing the screen position of the sprite and
    //the last tap screen position
    private void ChangePetFacingDirection(){
        if(destinationPoint.x > petSprite.transform.position.x){
            petSprite.GetComponent<tk2dSprite>().FlipX = true;
			shadowObject.transform.localPosition = new Vector3(0.6f, 
                shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }else{
            petSprite.GetComponent<tk2dSprite>().FlipX = false;
			shadowObject.transform.localPosition = new Vector3(-0.6f, 
                shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }
    }
}
