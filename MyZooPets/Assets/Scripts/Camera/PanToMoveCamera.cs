using UnityEngine;
using System.Collections;

//---------------------------------------------------
// PanToMoveCamera
//    Attach this script to main camera.
//    Panning left or right will move the camera x position right or left, respectively.
//    Swiping Left or right will snap the camera x position right or left, respectively.
//---------------------------------------------------

public class PanToMoveCamera : MonoBehaviour{
    public float minNormalizedPanDistance = 0.05f; //min normalized panning distance
    public int numOfPartitions = 4; //number of partitions allowed
    public int firstPartition = -1; //Set this to negative numbers if you want to open a partition
                                    //on the left of the starting partition(always 0)
    public int lastPartition = 2;
    public float partitionOffset = 80.0f; //How big each partition is in world position

    private Vector2 startTouchPos; //Position of touch when finger touches the screen
    private Vector2 currentTouchPos; //Position of touch right now
    public int currentPartition = 0;
    private float startTime; //Time at when finger touches screen
    private RoomDirection panDirection; //direction of the last finger gesture
    private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
    private bool touchCancelled = false; //True: touch shouldn't be handled
    public float maxSwipeTime = 0.3f; //Swipe gesture needs to be faster than maxSwipeTime
	public float panDistanceToChange = 0.5f;	// distance to pan before the camera will snap to the next partition
	
	// link to hud animator
	public HUDAnimator scriptHudAnim;

    private Camera nguiCamera; 
    private Camera mainCamera;

    // Use this for initialization
    void Start () {
       mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
       int layerNGUI = LayerMask.NameToLayer("NGUI");
       nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);
        
       D.Assert(nguiCamera != null, "NGUI camera not found");
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
                    if(!ClickManager.Instance.CanRespondToTap() || CameraUtils.IsTouchingNGUI(nguiCamera, startTouchPos) || CameraUtils.IsTouchingPet(mainCamera, startTouchPos))
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
					bool bMovingLeft = currentTouchPos.x < startTouchPos.x && currentPartition != lastPartition;
					bool bMovingRight = currentTouchPos.x > startTouchPos.x && currentPartition != firstPartition;
				
					if ( bMovingLeft || bMovingRight ) {
						panDirection = bMovingLeft ? RoomDirection.Left : RoomDirection.Right;
					
					    if(normalizedTouchPosX >= minNormalizedPanDistance) {
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = bMovingLeft ? currentPosX + normalizedPartitionX : currentPosX - normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
					}
                break;

                case TouchPhase.Ended:
					float swipeTime = Time.time - startTime;
					int nTargetPartition = GetTargetPartition( 1, panDirection );
					if ( CanMoveToPartition( nTargetPartition, panDirection, swipeTime ) )
						ChangePartition( nTargetPartition );
	
					// then snap the camera to the partition
                    SnapCamera();
                    
                    touchCancelled = false;
                break;
            }
        }
    }    

	///////////////////////////////////////////////////
    // CanMoveToPartition() 
    // The user has attempted to initiate a change in
	// partition; this function makes sure that it is
	// a legal move.
    /////////////////////////////////////////////////// 
	private bool CanMoveToPartition( int nTargetPartition, RoomDirection panDirection, float swipeTime ) {	
		// first check that the user siped long and hard enough
		if ( swipeTime > 0 ) {
			bool bSwipedRight = ( swipeTime <= maxSwipeTime && normalizedTouchPosX >= minNormalizedPanDistance ) || normalizedTouchPosX >= panDistanceToChange;
	        if( bSwipedRight == false ) 			
				return false;
		}
			
		// then check to make sure the gating manager is okay with the move
		if ( GatingManager.Instance.CanEnterRoom( currentPartition, panDirection ) == false )
			return false;
			
		// also check to make sure that the HUD animator is not animating
		if ( scriptHudAnim && scriptHudAnim.AreSpawnedSprites() )
			return false;
		
		// if we get here, the move is valid
		return true;
	}

    ///////////////////////////////////////////////////
    // CheckArrowKeys() 
    // Checks arrow key input for moving around the
    // area.  Only use for UNITY_EDITOR
    /////////////////////////////////////////////////// 
    private void CheckArrowKeys()
    {
        if( Input.GetKeyDown( KeyCode.RightArrow ) ) {
			if ( CanMoveToPartition( GetTargetPartition( 1, RoomDirection.Left ), RoomDirection.Left, -1 ) ) {
				ChangePartition( GetTargetPartition( 1, RoomDirection.Left ) );
	            SnapCamera();
			}
		}
        else if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
			if ( CanMoveToPartition( GetTargetPartition( 1, RoomDirection.Right ), RoomDirection.Right, -1 ) ) {
				ChangePartition( GetTargetPartition( 1, RoomDirection.Right ) );
	            SnapCamera();       
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
	// SnapCamera()
	// Snaps the camera to the current partition.
	///////////////////////////////////////////		
	private void SnapCamera() {
        Hashtable optional = new Hashtable();
        float moveTo = 0;

        moveTo = partitionOffset * currentPartition;
        LeanTween.moveX(gameObject, moveTo, 0.25f, optional);
        normalizedTouchPosX = 0;		
	}
	
	///////////////////////////////////////////
	// ChangePartition()
	// Changes the current partition by nMoves
	// in eSwipeDirection (if it's legal).
	///////////////////////////////////////////		
	private void ChangePartition( int nTargetPartition) {
		// check to make sure the move is legal (i.e. within bounds)
		if ( nTargetPartition >= firstPartition && nTargetPartition <= lastPartition ) {
			int nOldPartition = currentPartition;
			currentPartition = nTargetPartition;
			
			GatingManager.Instance.EnteredRoom( nOldPartition, currentPartition );
		}
	}
	
	///////////////////////////////////////////
	// GetTargetPartition()
	// Given a direction and a distance, what
	// partition is the target?
	///////////////////////////////////////////		
	private int GetTargetPartition( int nMoves, RoomDirection eSwipeDirection ) {
		int nChange = eSwipeDirection == RoomDirection.Left ? nMoves : -nMoves;
		int nTarget = currentPartition + nChange;
		
		return nTarget;
	}
}