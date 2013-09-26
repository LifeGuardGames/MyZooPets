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
    private Direction panDirection; //direction of the last finger gesture
    private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
    private bool touchCancelled = false; //True: touch shouldn't be handled
    public float maxSwipeTime = 0.3f; //Swipe gesture needs to be faster than maxSwipeTime
	public float panDistanceToChange = 0.5f;	// distance to pan before the camera will snap to the next partition

    private Camera nguiCamera; 
    private Camera mainCamera;
	private enum Direction{
	    Left,
	    Right
	}
	

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

                    // Cancel touch if finger is touching undesirable objects while panning
                    if(CameraUtils.IsTouchingNGUI(nguiCamera, startTouchPos) || CameraUtils.IsTouchingPet(mainCamera, startTouchPos))
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
						panDirection = bMovingLeft ? Direction.Left : Direction.Right;
					
					    if(normalizedTouchPosX >= minNormalizedPanDistance) {
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = bMovingLeft ? currentPosX + normalizedPartitionX : currentPosX - normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
					}
                break;

                case TouchPhase.Ended:
					// if the user swiped quick enough (or panned long enough), change the partition
                    float swipeTime = Time.time - startTime;
                    if( 	( swipeTime <= maxSwipeTime && normalizedTouchPosX >= minNormalizedPanDistance) || 
							( normalizedTouchPosX >= panDistanceToChange ) )
								ChangePartition( 1, panDirection );
	
					// then snap the camera to the partition
                    SnapCamera();
                    
                    touchCancelled = false;
                break;
            }
        }
    }    

    ///////////////////////////////////////////////////
    // CheckArrowKeys() 
    // Checks arrow key input for moving around the
    // area.  Only use for UNITY_EDITOR
    /////////////////////////////////////////////////// 
    private void CheckArrowKeys()
    {
        if( Input.GetKeyDown( KeyCode.RightArrow ) ) {
			ChangePartition( 1, Direction.Left );
            SnapCamera();
		}
        else if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
			ChangePartition( 1, Direction.Right );
            SnapCamera();       
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
	private void ChangePartition( int nMoves, Direction eSwipeDirection ) {
		// get the partition limit to check and numerical change in partition based on the direction
		int nCheck = eSwipeDirection == Direction.Left ? lastPartition : firstPartition;
		int nChange = eSwipeDirection == Direction.Left ? nMoves : -nMoves;
		
		// check to make sure the move is legal (i.e. within bounds)
		if ( 	( 	eSwipeDirection == Direction.Left && currentPartition + nChange <= nCheck )  ||
				( 	eSwipeDirection == Direction.Right && currentPartition + nChange >= nCheck ) )
					currentPartition += nChange;
	}
}