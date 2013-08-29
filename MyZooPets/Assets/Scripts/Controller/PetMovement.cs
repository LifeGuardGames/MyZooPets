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
    public bool allowPetMoveAround;
    public Transform area1;
    public Transform area2;
    public Transform area3;

    private GameObject petSprite; //Pet sprite
    private tk2dSpriteAnimator anim; //2D sprite animator
	public GameObject shadowObject;		// The shadow of the pet

    private Vector3 destinationPoint; //destination that the pet is going to move to
    private TapItem tapItem; //Tap gesture
	private bool moving; //Is Pet moving now or not

	private float moveToX;
	private float moveToZ;

    void Awake(){
        if (mainCamera == null){
            mainCamera = GameObject.Find("Main Camera").camera;
        }
        petSprite = GameObject.Find("SpritePet");
        tapItem = GetComponent<TapItem>();
        anim = petSprite.GetComponent<tk2dSpriteAnimator>();
    }

    void Start(){
       destinationPoint = petSprite.transform.position;
       tapItem.OnTap += MovePet;
       //how often does pet walk by himself.
        if(allowPetMoveAround) InvokeRepeating("PetWalkAround",5f,5f);
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

    //Event listener for tap on walkable area
    private void MovePet(){
        print("move pet");
        // if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
        if (!ClickManager.Instance.CanRespondToTap()) return;
        MovePet(Camera.main.ScreenPointToRay(tapItem.lastTapPosition));    
    }

    //Check if the touch is in walkable area then move/animate pet
    private void MovePet(Ray myRay){
        RaycastHit hit;
        //Debug.DrawRay(myRay.origin, myRay.direction * 50, Color.green, 50f);
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == area1.collider || 
                hit.collider == area2.collider ||
                hit.collider == area3.collider){
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

    //need to check if the pet moved out of the walk area
	private void PetWalkAround(){
		// //Get a random value for pet to move.
		// float ran1 = Random.value;
		// float ran2 = Random.value;
		// float ran3 = Random.value;
		// float ran4 = Random.value;
		// if(ran1 < 0.5) ran2 = -ran2;
		// if(ran3 < 0.5) ran4 = -ran4;
		// moveToX = petSprite.transform.position.x + ran2 *10;
		// moveToZ = petSprite.transform.position.z + ran4 *10;

		// RaycastHit hit;
		// Physics.Raycast(new Vector3(moveToX, planeCenter.transform.position.y ,
  //           moveToZ),new Vector3(0,-100,0),out hit);
		// if (hit.collider == planeCenter.collider || planeRight.collider){
  //           if(moving == false){
  //              	destinationPoint = hit.point;
		// 		moving = true;
		// 	}
  //       }
	}

    //Decides when to flip sprite by comparing the screen position of the sprite and
    //the last tap screen position
    private void ChangePetFacingDirection(){
        Vector3 petScreenPos = mainCamera.WorldToScreenPoint(petSprite.transform.position);
        if(tapItem.lastTapPosition.x > petScreenPos.x){
            petSprite.GetComponent<tk2dSprite>().FlipX = true;
			shadowObject.transform.localPosition = new Vector3(0.6f, shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }else{
            petSprite.GetComponent<tk2dSprite>().FlipX = false;
			shadowObject.transform.localPosition = new Vector3(-0.6f, shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }
    }

	
}
