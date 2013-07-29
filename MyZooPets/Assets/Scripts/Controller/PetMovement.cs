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

public class PetMovement : MonoBehaviour {

    private Transform planeCenter;
    private Transform planeRight;

    private GameObject petSprite;
    private Vector3 destinationPoint;
    private TapItem tapItem;
    public Camera mainCamera;
    public bool allowPetMoveAround;
	private bool moving;

	private float moveToX;
	private float moveToZ;

    void Awake(){
        if (mainCamera == null){
            mainCamera = GameObject.Find("Main Camera").camera;
        }
        petSprite = GameObject.Find("SpritePet");
        tapItem = GetComponent<TapItem>();
        planeCenter = transform.Find("planeCenter");
        planeRight = transform.Find("planeRight");
    }

    void Start(){
       destinationPoint = petSprite.transform.position;
       tapItem.OnTap += MovePet;
       //how often does pet walk by himself.
        if(allowPetMoveAround) InvokeRepeating("PetWalkAround",5f,5f);
    }

	public void movePetWithCamera(){
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(600, 200, 0));
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			if (hit.collider == planeCenter.collider || planeRight.collider){
        		if(moving == false){
					destinationPoint = hit.point;
					moving = true;
				}
			}
		}
	}

    void MovePet(){
        // if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
        if (!ClickManager.CanRespondToTap()) return;

        Ray myRay = Camera.main.ScreenPointToRay(tapItem.lastTapPosition);
        RaycastHit hit;
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == planeCenter.collider || planeRight.collider){
            	if(moving == false){
                	destinationPoint = hit.point;
					moving = true;
				}
            }
        }
        ChangePetFacingDirection();
    }

    //need to check if the pet moved out of the walk area
	void PetWalkAround(){
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

    void ChangePetFacingDirection(){
        if (destinationPoint.x > petSprite.transform.position.x){
            // face right
            petSprite.GetComponent<tk2dSprite>().FlipX = false;
        }
        else {
            // face left
            petSprite.GetComponent<tk2dSprite>().FlipX = true;
        }
    }

	// Update is called once per frame
	void Update () {
        if (petSprite != null){
            if (ClickManager.CanRespondToTap()){
                petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,
                    destinationPoint,5f * Time.deltaTime);
            }
            if(petSprite.transform.position == destinationPoint) moving = false;
        }
	}
}
