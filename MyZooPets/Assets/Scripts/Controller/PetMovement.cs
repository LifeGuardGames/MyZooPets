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

	public ParticleSystem particleMove;
	public string soundMoveKey;
	public float fShadow = .6f;

	// Used for 2D room pet movement, height screen ratio in normal room walking
	private float movementStaticHeightRatio;	

	// Used for 2D room pet movement, height screen point in normal room walking
	private float movementStaticScreenY;
	public  bool offScreen = true;

	// Used for 2D pet movement, this will be the z depth for all pet placement
	// Useful for zoomed in modes (accessories) where camera is zoomed in
	private float movementStaticZ;	
	public float MovementStaticZ{
		get{ return MovementStaticZ; }
	}
	public bool gateDestroyed = false; // bool to toggle movement after a pet is 
	public bool canMove;
	private Vector3 destinationPoint; //destination that the pet is going to move to
	public bool moving; //Is Pet moving now or not
	private Camera mainCamera;
	private Camera nguiCamera; //Use to check if user is clicking on NGUI element. Pet shouldn't
	//be moved when clicking on NGUI
	private PanToMoveCamera scriptPan; //script that pan the camera
	
	// how fast the pet moves
	private float normalSpeed;
	private float sickSpeed;
	private float verySickSpeed;
	public bool movingToAccessory;

	void Awake(){
		GatingManager.OnDestroyedGate += MoveToCenter;
		// set up camera variables
		scriptPan = CameraManager.Instance.PanScript;
		mainCamera = CameraManager.Instance.CameraMain;
		
		int layerNGUI = LayerMask.NameToLayer("NGUI");
		nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);
		
		// get speed from constants
		normalSpeed = Constants.GetConstant<float>("NormalMoveSpeed");
		sickSpeed = Constants.GetConstant<float>("SickMoveSpeed");
		verySickSpeed = Constants.GetConstant<float>("VerySickMoveSpeed");

		// Get the constant height ratio in screens of 2D pet movement
		movementStaticHeightRatio = Constants.GetConstant<float>("StaticMovementHeightRatio");
	}

	void Start(){
		canMove = true;
		destinationPoint = petSprite.transform.position;
		scriptPan.OnPartitionChanged += MovePetWithCamera;

		// Get the screen height for pet walking in regular room mode
		movementStaticScreenY = Screen.height * movementStaticHeightRatio;

		// Get the 3D Z-value where the pet will be locked to
		Ray initRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2f, movementStaticScreenY));
		// Debug.DrawRay(initRay.origin, initRay.direction * 50, Color.green, 5000f);

		RaycastHit hit;
		int layerMask = 1 << 11;	// Only tell it to detect the ground layer
		if(Physics.Raycast(initRay, out hit, Mathf.Infinity, layerMask)){
			foreach(Collider walkingPathCollider in walkingPathColliders){
				// This is the point where the ratio ray will hit the floor
				if(hit.collider == walkingPathCollider){	// Assume one floor for now
					movementStaticZ = hit.point.z;
					break;
				}
				else{
					Debug.LogWarning("Raycast did not hit any floor");
				}
			}
		}

		// Make sure the pet is in the correct Z-plane
		Vector3 petPos = petSprite.transform.position;
		petSprite.transform.position = new Vector3(petPos.x, petPos.y, movementStaticZ);
	}

	private void MoveToCenter(object sender, EventArgs args){
		if(petSprite == null){
			petSprite = GameObject.Find("Pet");
		}
		canMove = true;
		gateDestroyed = true;
		PetAnimationManager.Instance.Flipping();
		MovePet(new Vector3(petSprite.transform.position.x + 15,petSprite.transform.position.y,petSprite.transform.position.z));	
	}

	// Update is called once per frame
	void Update(){
		if(canMove){
			this.gameObject.GetComponentInChildren<MeshCollider>().enabled = true;
		}
		else{
			this.gameObject.GetComponentInChildren<MeshCollider>().enabled = false;
		}
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
			else if(moving && gateDestroyed){
				petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,
				                                                   destinationPoint, normalSpeed * Time.deltaTime);
			}
			else if (movingToAccessory){
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
				if(movingToAccessory){
					movingToAccessory = false;
				}
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
		if(DecoInventoryUIManager.Instance && DecoInventoryUIManager.Instance.IsOpen()){
			return;
		}

		AudioManager.Instance.PlayClip(soundMoveKey);
		MovePet(Camera.main.ScreenPointToRay(new Vector3(gesture.Position.x, movementStaticScreenY, 0)));    
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
			MovePet(mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, movementStaticScreenY, 0)));
		}
	}
	
	public void StopMoving(bool stopAnimation = true){
		moving = false;
		offScreen = true;
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
					particleMove.transform.position = hit.point;
					particleMove.Play();
				}
			}
		}
	}

	public void MovePetFromAccessory(Vector3 raycastHitPosition){
		destinationPoint = raycastHitPosition;
		movingToAccessory = true;
		// tell the pet animator script to start moving (but only if we aren't already moving)
		if(!moving){
			PetAnimationManager.Instance.StartWalking();
		}
		moving = true;	
		
		// if the pet is not visible on the screen, we want to cheat and transport the pet *just* off screen so that it doesn't
		// take so long for the pet to move to its new destination.
		if(!petSprite.renderer.isVisible){
			if(offScreen){
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
				offScreen = false;
			}
		}
		
		ChangePetFacingDirection();
	}
	
	public void MovePet(Vector3 raycastHitPosition){
		destinationPoint = raycastHitPosition;
		// tell the pet animator script to start moving (but only if we aren't already moving)
		if(!moving){
			PetAnimationManager.Instance.StartWalking();
		}
		moving = true;	
		
		// if the pet is not visible on the screen, we want to cheat and transport the pet *just* off screen so that it doesn't
		// take so long for the pet to move to its new destination.
		if(!petSprite.renderer.isVisible){
			if(offScreen){
				// Get the point right off screen in viewport coordinates
				float viewportOffsetX = Constants.GetConstant<float>("ViewportOffsetX");
				// Add 1 to the viewport offset if the pet coming from left or right
				float locationDifference = raycastHitPosition.x - petSprite.transform.position.x;
				viewportOffsetX = locationDifference < 0 ? 1 + viewportOffsetX : -viewportOffsetX;
				// also, the viewport y varies and is based on where the player is moving to
				Vector3 viewPortPointOfRaycastLoc = Camera.main.WorldToViewportPoint(raycastHitPosition);
				float startLocationY = viewPortPointOfRaycastLoc.y;
				// change the y and z because we really only want the x.  if we don't change the z the pet kind of appears too big
				Vector3 targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(viewportOffsetX, startLocationY, raycastHitPosition.z));

				targetPosition.y = raycastHitPosition.y;
				targetPosition.z = raycastHitPosition.z;
			
				// transport the pet to that point
				petSprite.transform.position = targetPosition;
				offScreen = false;
			}
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
