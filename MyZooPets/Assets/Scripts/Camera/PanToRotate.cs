using UnityEngine;
using System.Collections;

/*
    Attach this script to main camera. 
    Panning left or right will rotate the camera right or left, respectively.
    Currently there are five partitions and each partition can be enabled/disabled
    panning speed and snapping speed can be adjusted in the inspector
*/
public class PanToRotate : MonoBehaviour {
	/*
    public float panSpeed = 0.5f; //the speed that you want the camera to rotate while panning
    public float snapSpeed = 0.5f; //the speed that you want the camera to snap back when touch ends
    public float minPanDistance = 80.0f; //min distance required for panning to be recognized
    
    private Vector2 startTouchPos; //position of the touch when finger touches the screen
    private bool[] enabledPartitions = {true, true, true, false, false}; //is the partition accessible to user
    private float[] partitionAngles = {0, 72, 144, 216, 288}; //the camera angle for the partition
    private int rightLimit = 2; //id of the first partition that is locked on the right
    private int leftLimit = 0; //rotation can never rotate to the left of the first partition
    private int currentIndex = 0; //current partition
    private int numPartitions = 5; //total number of room partitions
    private Direction panDirection; //the direction of the last pan gesture
    private int partitionOffset = 20; //camera rotates if it's rotated +/- partitionOffset of the current partition angle
    private Hashtable snapOption1;
    private Hashtable snapOption2;
    private bool touchCancelled; //cancel touch detection if user click on NGUI first
    private Camera nguiCamera;
    private Camera mainCamera;
    private enum Direction{
        Left,
        Right
    }
	// Use this for initialization
	void Start () {
		numPartitions = enabledPartitions.Length;
		snapOption1 = new Hashtable();
		snapOption2 = new Hashtable();
		snapOption1.Add("ease", LeanTweenType.easeOutBack);
		snapOption1.Add("onCompleteTarget", PetMovement.Instance.gameObject);
		snapOption1.Add("onComplete", "MovePetWithCamera");
		snapOption2.Add("ease", LeanTweenType.easeOutBack);
		mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
		int layerNGUI = LayerMask.NameToLayer("NGUI");
		nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);

		if(nguiCamera == null){
			Debug.LogError("NGUI camera not found");
		}
	}
	
	//************************************************
	// CheckArrowKeys() 
	// Checks arrow key input for moving around the
	// area.  Only use for standalone and web player.
	//************************************************	
	private void CheckArrowKeys()
	{
		// check arrow keys
		if( Input.GetKeyDown( KeyCode.RightArrow ) )
			MoveCameraWithKey( Direction.Left );
		else if ( Input.GetKeyDown( KeyCode.LeftArrow ) )
			MoveCameraWithKey( Direction.Right );		
	}
	
	//************************************************
	// MoveCameraWithKey()
	// Snaps and moves the camera with arrow keys.
	//************************************************		
	private void MoveCameraWithKey( Direction eDirection ) {
		// get the index properly depending on which direction we're moving in
        int nIndex = eDirection == Direction.Left ? GetNextPartitionIndex() : GetPrevPartitionIndex();

        if( nIndex != -1 && enabledPartitions[nIndex] ) {
            float fRotateTo = partitionAngles[nIndex];	// get the rotation of the partition

            currentIndex = nIndex;						// update our current partition
			
            LeanTween.rotateY(gameObject, fRotateTo, snapSpeed, snapOption1);
        }		
	}

	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		CheckArrowKeys();
#endif
		
        if (Input.touchCount > 0) {
            Touch touch = Input.touches[0];
            switch (touch.phase) {
                case TouchPhase.Began:
                    startTouchPos = touch.position;                 
                    
                    //Cancel touch if finger is touching on undesirable objects
                    if(IsTouchingNGUI(startTouchPos) || IsTouchingPet(startTouchPos)) 
                        touchCancelled = true;
                break;
                case TouchPhase.Moved:
                    //Detect if finger is panning left or right
                    //if left rotate camera to the right else to the left
                    //camera can only rotate if the partition is enabled
                    if(touchCancelled) return;
                    Vector2 touchDeltaPosition = touch.deltaPosition;
                    float rotate = 0; 
                    if(touch.position.x < startTouchPos.x - minPanDistance){
						
                        panDirection = Direction.Left;
                        int nextIndex = GetNextPartitionIndex();
                        if(nextIndex != -1 && enabledPartitions[nextIndex]){
                            float temp = touchDeltaPosition.x * panSpeed;

                            //Check if camera is not rotating beyond the allowed partitions
                            float calculatedRotation = gameObject.transform.eulerAngles.y + (-temp);
                            if(calculatedRotation < partitionAngles[rightLimit]) rotate = temp;
                        }
                        
						//MoveCamera( Direction.Left, touch );
                    }else if(touch.position.x > startTouchPos.x + minPanDistance){
					
                        panDirection = Direction.Right;
                        int prevIndex = GetPrevPartitionIndex();
                        if(prevIndex != -1 && enabledPartitions[prevIndex]){
                            float temp = touchDeltaPosition.x * panSpeed;

                            //Check if camera is not rotating beyond the allowed partitions
                            float calculatedRotation = gameObject.transform.eulerAngles.y + (-temp);
                            if(calculatedRotation > partitionAngles[leftLimit]) rotate = temp;
                        }
                      
					//MoveCamera( Direction.Right, touch );
                    }
                    
                    gameObject.transform.Rotate(0, -rotate, 0);
                break;
                case TouchPhase.Ended:
                    if(touchCancelled){
                        touchCancelled = false;
                        return;
                    }

                    //When the panning ends decides which direction to snap the camera
                    if(panDirection.Equals(Direction.Left)) //panning left, so rotate right
                        SnapCameraToRight();
                    else if(panDirection.Equals(Direction.Right)) //panning right, so rotate left
                        SnapCameraToLeft();                        
                break;
            }
        }
	}

    //Rotate camera to right when touch is finished
    private void SnapCameraToRight(){
        int nextIndex = GetNextPartitionIndex();
        float rotateTo;
        Hashtable optional;
        if(nextIndex != -1 && enabledPartitions[nextIndex]){
            //if camera angle is closer to the angle of the next partition then
            //snap the camera to the next partition
            if(transform.eulerAngles.y > partitionAngles[currentIndex] + partitionOffset){
                rotateTo = partitionAngles[nextIndex];
                currentIndex = nextIndex;
                optional = snapOption1;
            }else{ //snap camera back to the current partition
               rotateTo = partitionAngles[currentIndex]; 
               optional = snapOption2;
            }
            LeanTween.rotateY(gameObject, rotateTo, snapSpeed, optional);
        }
    }

    //Rotate camera to left when touch is finished
    private void SnapCameraToLeft(){
        int prevIndex = GetPrevPartitionIndex();
        float rotateTo;
        Hashtable optional;
        if(prevIndex != -1 && enabledPartitions[prevIndex]){
            //if the camera angle is closer to the angle of the previous partition then
            //snap the camera to the previous partition
            if(transform.eulerAngles.y < partitionAngles[currentIndex] - partitionOffset){
                rotateTo = partitionAngles[prevIndex];
                currentIndex = prevIndex;
                optional = snapOption1;
            }else{ //snap the camera back to current partition
                rotateTo = partitionAngles[currentIndex];
                optional = snapOption2;
            }
            LeanTween.rotateY(gameObject, rotateTo, snapSpeed, optional);
        }
    }

    //Return index of the partition on the right.
    //Return -1 if the current partition is the last
    private int GetNextPartitionIndex(){
        int nextIndex = -1;
        if(currentIndex != numPartitions - 1){
            nextIndex = currentIndex + 1;
        }
        return nextIndex;
    }

    //Return index of the partition on the left
    //Return -1 if the current partition is the last
    private int GetPrevPartitionIndex(){
        int prevIndex = -1;
        if(currentIndex != 0){
            prevIndex = currentIndex - 1;
        }
        return prevIndex;
    }

    //True: if finger touches NGUI 
    private bool IsTouchingNGUI(Vector2 screenPos){
        Ray ray = nguiCamera.ScreenPointToRay (screenPos);
        RaycastHit hit;
        int layerMask = 1 << 10;
        bool isOnNGUILayer = false;
        // Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            isOnNGUILayer = true;
        }
        return isOnNGUILayer;
    }

    //True; if finger touches default layer
    private bool IsTouchingPet(Vector2 screenPos){
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        int layerMask = 1 << 0;
        bool isOnPet = false;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
            if(hit.collider.name == "Head" || hit.collider.name == "Tummy"){
                isOnPet = true;
            }
        }
        return isOnPet;
    }
	*/
}
