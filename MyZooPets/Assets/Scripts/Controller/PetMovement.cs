using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PetMovement : Singleton<PetMovement>{
	//=======================Events========================
	public EventHandler<EventArgs> OnReachedDest;   // when the pet reaches its destination
	//=====================================================     

	public List<Collider> walkingPathColliders; //Areas that the pet is allowed to move
	public GameObject petSprite;

	// sound for when the pet moves
	public string strSoundMove;
	public float fShadow = .6f;
	private Vector3 destinationPoint; //destination that the pet is going to move to
	private bool moving; //Is Pet moving now or not
	private Camera mainCamera;
	private Camera nguiCamera; //Use to check if user is clicking on NGUI element. Pet shouldn't
	//be moved when clicking on NGUI
	private PanToMoveCamera scriptPan; //script that pan the camera
	
	// how fast the pet moves
	private float normalSpeed;
	private float sickSpeed;
	private float verySickSpeed;

	void Awake(){
		// set up camera variables
		scriptPan = CameraManager.Instance.GetPanScript();
		mainCamera = CameraManager.Instance.cameraMain;
		
		int layerNGUI = LayerMask.NameToLayer("NGUI");
		nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);
		
		// get speed from constants
		normalSpeed = Constants.GetConstant<float>("NormalMoveSpeed");
		sickSpeed = Constants.GetConstant<float>("SickMoveSpeed");
		verySickSpeed = Constants.GetConstant<float>("VerySickMoveSpeed");
	}

	void Start(){
		destinationPoint = petSprite.transform.position;
		scriptPan.OnPartitionChanged += MovePetWithCamera;
	}

	// Update is called once per frame
	void Update(){
		if(moving && petSprite != null){
			//move the pet location if allowed
			if(ClickManager.Instance.CanRespondToTap(this.gameObject, ClickLockExceptions.Moving)){ 

				PetMoods mood = DataManager.Instance.GameData.Stats.GetMoodState();             
				PetHealthStates health = DataManager.Instance.GameData.Stats.GetHealthState();
				float movementSpeed = normalSpeed;

				//show pet movement down if pet is sick
				if(health == PetHealthStates.Sick || mood != PetMoods.Happy)
					movementSpeed = sickSpeed;
				else if(health == PetHealthStates.VerySick)
					movementSpeed = verySickSpeed;

				petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,
                    destinationPoint, movementSpeed * Time.deltaTime);
			}
			else
				StopMoving();

			//when the sprite reaches destination. stop transform and animation
			if(petSprite.transform.position == destinationPoint){
				// send out an event because the pet has reached their destination
				if(OnReachedDest != null)
					OnReachedDest(this, EventArgs.Empty);
				
				StopMoving();
			}
		}
	}

	//---------------------------------------------------
	// ProcessTap()
	// This function is called by PetMovementListener when user taps on an area
	// that the pet can move to.
	//---------------------------------------------------
	public void ProcessTap(TapGesture gesture){
		// if the player is in a gated room, moving on tap is not allowed
		if(GatingManager.Instance.IsInGatedRoom())
			return;
        
		// if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
		bool isPetAnimatorBusy = PetAnimationManager.Instance.IsBusy;
		if(!ClickManager.Instance.CanRespondToTap(this.gameObject, ClickLockExceptions.Moving) || 
			IsTouchingNGUI(gesture.Position) || isPetAnimatorBusy)
			return;
		
		// bit of a hack...remove if this causes any issues -- prevent pet movement if the edit decos UI is open
		if(EditDecosUIManager.Instance && EditDecosUIManager.Instance.IsOpen())
			return;
       
		AudioManager.Instance.PlayClip(strSoundMove);

		MovePet(Camera.main.ScreenPointToRay(gesture.Position));    
	}

	//---------------------------------------------------
	// MovePetWithCamera()
	//Pet will follow the camera when the partition has been changed
	//---------------------------------------------------
	public void MovePetWithCamera(object sender, PartitionChangedArgs arg){
		bool hasActiveGate = GatingManager.Instance.HasActiveGate(arg.newPartition);
		if(!hasActiveGate){		
			// first add a temporary exception so the pet can move freely
			ClickManager.Instance.AddTemporaryException(ClickLockExceptions.Moving);
			
			//Transform pet position to screen point first so we can move the pet to the right y position
			Vector2 petPosInScreenPoint = mainCamera.WorldToScreenPoint(petSprite.transform.position);
			MovePet(mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, petPosInScreenPoint.y, 0)));
		}
	}
	
	public void StopMoving(bool stopAnimation = true){
		moving = false;

		if(stopAnimation)
			PetAnimationManager.Instance.StopWalking();
	}
	
	//---------------------------------------------------
	// MovePet()
	// Check if the touch is in walkable area then move/animate pet
	//---------------------------------------------------
	private void MovePet(Ray myRay){
		RaycastHit hit;
		// Debug.DrawRay(myRay.origin, myRay.direction * 50, Color.green, 50f);

		if(Physics.Raycast(myRay, out hit)){
			foreach(Collider walkingPathCollider in walkingPathColliders){
				if(hit.collider == walkingPathCollider){
					MovePet(hit.point);
				}
			}
		}
	}
	
	public void MovePet(Vector3 raycastHitPosition){
		destinationPoint = raycastHitPosition;
		
		// tell the pet animator script to start moving (but only if we aren't already moving)
		if(!moving)
			PetAnimationManager.Instance.StartWalking();
		
		moving = true;	
		
		// if the pet is not visible on the screen, we want to cheat and transport the pet *just* off screen so that it doesn't
		// take so long for the pet to move to its new destination.
		if(!petSprite.renderer.isVisible){
			// get the point right off screen
			float startingLocationX = Constants.GetConstant<float>("FromX");
			float locationDifference = raycastHitPosition.x - petSprite.transform.position.x;
			startingLocationX = locationDifference < 0 ? 1 + startingLocationX : -startingLocationX; 				// the point varies if the pet is coming from the right or left
			
			// also, the viewport y varies and is based on where the player is moving to
			Vector3 viewPortPointOfRaycastLoc = Camera.main.WorldToViewportPoint(raycastHitPosition);
			float startLocationY = viewPortPointOfRaycastLoc.y;
			
			// change the y and z because we really only want the x.  if we don't change the z the pet kind of appears too big
			Vector3 targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(startingLocationX, startLocationY, raycastHitPosition.z));
			targetPosition.y = raycastHitPosition.y;
			targetPosition.z = raycastHitPosition.z;
			
			// transport the pet to that point
			petSprite.transform.position = targetPosition;
		}
		
		ChangePetFacingDirection();
	}
	
	
	//---------------------------------------------------
	// ChangePetFacingDirection()
	// Decides when to flip sprite by comparing the screen position of the sprite and
	// the last tap screen position
	//---------------------------------------------------
	private void ChangePetFacingDirection(){
		// if the pet hasn't actually moved, then we don't have to worry about flipping anything
		if(destinationPoint == petSprite.transform.position) 
			return;
		
		if(destinationPoint.x > petSprite.transform.position.x){
			PetAnimationManager.Instance.Flip(true);
		}
		else{
			PetAnimationManager.Instance.Flip(false);
		}
	}

	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;

		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
}
