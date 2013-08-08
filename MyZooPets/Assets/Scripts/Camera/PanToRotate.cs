using UnityEngine;
using System.Collections;

/*
    Attach this script to main camera. 
    Panning left or right will rotate the camera right or left, respectively.
    Currently there are five partitions and each partition can be enabled/disabled
    panning speed and snapping speed can be adjusted in the inspector
*/
public class PanToRotate : MonoBehaviour {
    public float panSpeed = 0.5f; //the speed that you want the camera to rotate while panning
    public float snapSpeed = 0.5f; //the speed that you want the camera to snap back when touch ends
    public float minPanDistance = 40.0f; //min distance required for panning to be recognized
    private Vector2 startTouchPos; //position of the touch when finger touches the screen
    private bool[] enabledPartitions = {true, true, true, false, false}; //is the partition accessible to user
    private float[] partitionAngles = {0, 72, 144, 216, 288}; //the camera angle for the partition
    private int currentIndex = 0; //current partition
    private int numPartitions = 5; //total number of room partitions
    private Direction panDirection; //the direction of the last pan gesture
    private int partitionOffset = 20; //camera rotates if it's rotated +/- partitionOffset of the current partition angle
    private Hashtable snapOption;
    private bool touchCancelled; //cancel touch detection if user click on NGUI first
    private Camera NGUICamera;
    private Camera mainCamera;
    // private int layerNGUI; //layer that NGUI is on
    private enum Direction{
        Left,
        Right
    }
	// Use this for initialization
	void Start () {
	   numPartitions = enabledPartitions.Length;
       snapOption = new Hashtable();
       snapOption.Add("ease", LeanTweenType.easeOutBack);
       mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
       int layerNGUI = LayerMask.NameToLayer("NGUI");
        NGUICamera = NGUITools.FindCameraForLayer(layerNGUI);
        if (NGUICamera == null){
            Debug.LogError("NGUI camera not found!");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0) {
            Touch touch = Input.touches[0];
            switch (touch.phase) {
                case TouchPhase.Began:
                    startTouchPos = touch.position;                 
                    if(IsTouchingNGUI(startTouchPos)) touchCancelled = true;
                break;
                case TouchPhase.Ended:
                    if(touchCancelled){
                        touchCancelled = false;
                        return;
                    }
                    //When the panning ends decides which direction to snap the camera
                    if(panDirection.Equals(Direction.Left)){ //panning left, so rotate right
                        int nextIndex = GetNextPartitionIndex();
                        float rotateTo;
                        if(nextIndex != -1 && enabledPartitions[nextIndex]){
                            if(transform.eulerAngles.y > partitionAngles[currentIndex] + partitionOffset){
                                rotateTo = partitionAngles[nextIndex];
                                currentIndex = nextIndex;
                            }else{
                               rotateTo = partitionAngles[currentIndex]; 
                            }
                            LeanTween.rotateY(gameObject, rotateTo, snapSpeed, snapOption);
                        }
                    }else if(panDirection.Equals(Direction.Right)){ //panning right, so rotate left
                        int prevIndex = GetPrevPartitionIndex();
                        float rotateTo;
                        if(prevIndex != -1 && enabledPartitions[prevIndex]){
                            if(transform.eulerAngles.y < partitionAngles[currentIndex] - partitionOffset){
                                rotateTo = partitionAngles[prevIndex];
                                currentIndex = prevIndex;
                            }else{
                                rotateTo = partitionAngles[currentIndex];
                            }
                            LeanTween.rotateY(gameObject, rotateTo, snapSpeed, snapOption);
                        }
                    }
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
                            rotate = touchDeltaPosition.x * panSpeed;
                        }
                    }else if(touch.position.x > startTouchPos.x + minPanDistance){
                        panDirection = Direction.Right;
                        int prevIndex = GetPrevPartitionIndex();
                        if(prevIndex != -1 && enabledPartitions[prevIndex]){
                            rotate = touchDeltaPosition.x * panSpeed;
                        }
                    }
                    gameObject.transform.Rotate(0, -rotate, 0);
                break;
            }
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
        Ray ray = NGUICamera.ScreenPointToRay (screenPos);
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
    private bool IsTouchingDefaultLayer(Vector2 screenPos){
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        int layerMask = 1 << 0;
        bool isOnDefaultLayer = false;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
            isOnDefaultLayer = true;;
        }
        return isOnDefaultLayer;
    }
}
