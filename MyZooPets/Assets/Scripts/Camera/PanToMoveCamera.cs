using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PartitionChangedArgs : EventArgs{
	public int nOld;
	public int nNew;

	public PartitionChangedArgs( int nOld, int nNew ){
		this.nOld = nOld;
		this.nNew = nNew;
	}
}

//---------------------------------------------------
// PanToMoveCamera
//    Attach this script to main camera.
//    Panning left or right will move the camera x position right or left, respectively.
//    Swiping Left or right will snap the camera x position right or left, respectively.
//---------------------------------------------------

public class PanToMoveCamera : MonoBehaviour {
    //=======================Events========================
    public EventHandler<PartitionChangedArgs> OnPartitionChanged;   // when the partition has changed (and the camera has finished moving)
    public EventHandler<PartitionChangedArgs> OnPartitionChanging;  // when the partition is changing (i.e. camera is still moving)
    //========================================================

    public float minNormalizedPanDistance = 0.05f; //min normalized panning distance
    public int numOfPartitions = 4; //number of partitions allowed
    public int firstPartition = -1; //Set this to negative numbers if you want to open a partition
                                    //on the left of the starting partition(always 0)
    public int lastPartition = 2;
    public float partitionOffset = 80.0f; //How big each partition is in world position
    public int currentPartition = 0;
    public float maxSwipeTime = 0.3f; //Swipe gesture needs to be faster than maxSwipeTime
    public float panDistanceToChange = 0.5f;    // distance to pan before the camera will snap to the next partition

    private Vector2 startTouchPos; //Position of touch when finger touches the screen
    private Vector2 currentTouchPos; //Position of touch right now
    private float startTime; //Time at when finger touches screen
    private RoomDirection panDirection; //direction of the last finger gesture
    private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
    private bool touchCancelled = false; //True: touch shouldn't be handled
    private Camera nguiCamera; 
    private Camera mainCamera;

    void Awake(){
       mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
       int layerNGUI = LayerMask.NameToLayer("NGUI");
       nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);
       D.Assert(nguiCamera != null, "NGUI camera not found");
       Input.multiTouchEnabled = false;

       //Change the swipe setting if this is a lite build
       if(VersionManager.IsLite()){
            numOfPartitions = Constants.GetConstant<int>("LiteBedroomNumOfPartitions");
            firstPartition = Constants.GetConstant<int>("LiteBedroomFirstPartition");
            lastPartition = Constants.GetConstant<int>("LiteBedroomLastPartition");
       }
    }

    // Use this for initialization
    void Start () {
       //Move camera to the last saved partition
       LoadSceneData sceneData = DataManager.Instance.SceneData;
       if(sceneData != null)
            if(sceneData.LastScene == Application.loadedLevelName)
                SetCameraToPartition(sceneData.LastCameraPartition);
}
    
    void Update(){
#if UNITY_EDITOR
    CheckArrowKeys();
#endif 
        
        if(Input.touchCount > 0){
            Touch touch = Input.touches[0];
            switch(touch.phase){
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    startTime = Time.time;

                    // Cancel touch if finger is touching undesirable objects while panning or the click manager is locked
                    if(!ClickManager.Instance.CanRespondToTap(mainCamera.gameObject, ClickLockExceptions.Moving) || 
                        CameraUtils.IsTouchingNGUI(nguiCamera, startTouchPos) || 
                        CameraUtils.IsTouchingPet(mainCamera, startTouchPos))
                        touchCancelled = true;
                break;

                case TouchPhase.Moved:
                    if(touchCancelled) 
						return;
						
                    // Vector2 touchDeltaPosition = touch.deltaPosition;
                    currentTouchPos = touch.position;
                    float currentPosX = currentPartition * partitionOffset;
                    normalizedTouchPosX = GetNormalizedPosition();
				
					// see if we are moving in either direction
					bool movingLeft = currentTouchPos.x < startTouchPos.x && currentPartition != lastPartition;
					bool movingRight = currentTouchPos.x > startTouchPos.x && currentPartition != firstPartition;
				
					if (movingLeft || movingRight) {
						panDirection = movingLeft ? RoomDirection.Left : RoomDirection.Right;
					
						// before panning the camera, check to see if it's legal to move to the place where the player is panning
						int targetPartition = GetTargetPartition(1, panDirection);
						bool bCanMove = CanMoveToPartition( targetPartition, panDirection, -1 );
						if ( !bCanMove )
							return;
					
					    if(normalizedTouchPosX >= minNormalizedPanDistance) {
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = movingLeft ? currentPosX + normalizedPartitionX : currentPosX - normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
					}
                break;

                case TouchPhase.Ended:
					float swipeTime = Time.time - startTime;
					int targetPartition = GetTargetPartition( 1, panDirection );
					if(CanMoveToPartition(targetPartition, panDirection, swipeTime))
						ChangePartition(targetPartition);
					else
						SnapCamera(currentPartition);
	
                    touchCancelled = false;
                break;
            }
        }
    }    
	
	///////////////////////////////////////////
	// SnapCamera()
	// Snaps the camera to the current partition.
	///////////////////////////////////////////		
	private void SnapCamera(int oldPartition) {
        float moveTo = partitionOffset * currentPartition;
		
		// if the camera is actually already in this position, don't bother doing anything	
		if (gameObject.transform.position.x == moveTo)
			return;
		
		// prepare the hashtables for the camera snap callback
        Hashtable optional = new Hashtable();
		optional.Add ("onComplete", "OnCameraSnapped");
		optional.Add("onCompleteTarget", gameObject);
		
		Hashtable completeParamHash = new Hashtable();
		completeParamHash.Add("Old", oldPartition);			
		optional.Add("onCompleteParam", completeParamHash);
			
        LeanTween.moveX(gameObject, moveTo, 0.25f, optional);
        normalizedTouchPosX = 0;		
	}
	
	///////////////////////////////////////////
	// OnCameraSnapped()
	// Callback for when the camera is done
	// snapping.
	///////////////////////////////////////////		
	private void OnCameraSnapped(Hashtable hash) {
		int oldPartition = (int) hash["Old"];
		
		// if we were snapping back, don't send anything
		if ( oldPartition == currentPartition )
			return;

		// camera is done snapping, so send the partition changed callback
		if ( OnPartitionChanged != null )
			OnPartitionChanged( this, new PartitionChangedArgs( oldPartition, currentPartition ) );		
	}
	
	///////////////////////////////////////////
	// ChangePartition()
	// Changes the current partition by moves 
	// in swipeDirection (if it's legal).
	///////////////////////////////////////////		
	private void ChangePartition( int targetPartition) {
		// check to make sure the move is legal (i.e. within bounds)
		if (targetPartition >= firstPartition && targetPartition <= lastPartition){
			int oldPartition = currentPartition;
			currentPartition = targetPartition;
			
			// the partition changed, so snap the camera
			SnapCamera( oldPartition );
			
			// also send a callback that the partition is in the process of changing
			if ( OnPartitionChanging != null )
				OnPartitionChanging( this, new PartitionChangedArgs( oldPartition, currentPartition ) );			
		}
	}	

	///////////////////////////////////////////////////
    // CanMoveToPartition() 
    // The user has attempted to initiate a change in
	// partition; this function makes sure that it is
	// a legal move.
    /////////////////////////////////////////////////// 
	private bool CanMoveToPartition( int targetPartition, RoomDirection panDirection, float swipeTime ) {	
        bool retVal = true;

		// first check that the user siped long and hard enough
		if(swipeTime > 0) {
			bool swipedRight = (swipeTime <= maxSwipeTime && normalizedTouchPosX >= minNormalizedPanDistance) || 
                                normalizedTouchPosX >= panDistanceToChange;
	        if(swipedRight == false) 			
                retVal = false;
		}
			
		// then check to make sure the gating manager is okay with the move
		if(GatingManager.Instance.CanEnterRoom(currentPartition, panDirection) == false)
            retVal = false;
			
		// also check to make sure that the HUD animator is not animating
		if(HUDUIManager.Instance && HUDUIManager.Instance.hudAnimator && 
            HUDUIManager.Instance.hudAnimator.AreSpawnedSprites())
            retVal = false;
		
		// if the user is in deco mode and the room they are moving to has an active gate, illegal move
		if(EditDecosUIManager.Instance && EditDecosUIManager.Instance.IsOpen() && 
            GatingManager.Instance.HasActiveGate(targetPartition))
            retVal = false;
		
		// if the shop is open, no movement allowed
		if(StoreUIManager.Instance && StoreUIManager.Instance.IsOpen())
            retVal = false;
		
		// if we get here, the move is valid
		return retVal;
	}

    //This method can only be used in GameTutorial_SmokeIntro
    //It doesn't check click manager because we need the user to swipe left during
    //the tutorial. 
    public void TutorialSwipeLeft(){
        if ( CanMoveToPartition( GetTargetPartition( 1, RoomDirection.Left ), RoomDirection.Left, -1 ) ) {
            ChangePartition( GetTargetPartition( 1, RoomDirection.Left ) );
            }
    }

    ///////////////////////////////////////////////////
    // CheckArrowKeys() 
    // Checks arrow key input for moving around the
    // area.  Only use for UNITY_EDITOR
    /////////////////////////////////////////////////// 
    private void CheckArrowKeys() {
		// do a check here to see if the clickmanager can respond to movement, if it can't, don't move
		if ( !ClickManager.Instance.CanRespondToTap( mainCamera.gameObject, ClickLockExceptions.Moving ) )
			return;
			
        if( Input.GetKeyDown( KeyCode.RightArrow ) ) {
            if ( CanMoveToPartition( GetTargetPartition( 1, RoomDirection.Left ), RoomDirection.Left, -1 ) ) {
                ChangePartition( GetTargetPartition( 1, RoomDirection.Left ) );
                }
		}
        else if( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
			if ( CanMoveToPartition( GetTargetPartition( 1, RoomDirection.Right ), RoomDirection.Right, -1 ) ) {
				ChangePartition( GetTargetPartition( 1, RoomDirection.Right ) );       
			}
		}
    }

    ///////////////////////////////////////////////////
    // GetNormalizedPosition() 
    // Returns (from 0-1) how much of the screen the
	// gesture covered.
    /////////////////////////////////////////////////// 	
    private float GetNormalizedPosition(){
        //Get the different of the touch position now and the touch position at start
        float deltaTouchPosX = Mathf.Abs(currentTouchPos.x - startTouchPos.x);

        //Divide by screen width to get the normalize position;
        return deltaTouchPosX / Screen.width;
    }
	
	///////////////////////////////////////////
	// GetTargetPartition()
	// Given a direction and a distance, what
	// partition is the target?
	///////////////////////////////////////////		
	private int GetTargetPartition(int moves, RoomDirection swipeDirection) {
		int change = swipeDirection == RoomDirection.Left ?  moves : -moves;
		int target = currentPartition + change;
		
		return target;
	}

    ///////////////////////////////////////////
    // SetCameraToPartition()
    // Set the camera position to the partition position
    ///////////////////////////////////////////     
    private void SetCameraToPartition(int partition){
        currentPartition = partition;
        float cameraX = partition * partitionOffset;
        transform.position = new Vector3(cameraX, transform.position.y, transform.position.z);
    }
}