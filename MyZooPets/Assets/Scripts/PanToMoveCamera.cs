using UnityEngine;
using System.Collections;

/*
    Attach this script to main camera.
    Panning left or right will move the camera x position right or left, respectively.
*/
public class PanToMoveCamera : MonoBehaviour{
    public float minNormalizedPanDistance = 0.1f; //min normalized panning distance
    public int numOfPartitions = 3; //number of partitions allowed
    public float partitionOffset = 80.0f; //How big each partition is in world position

    private Vector2 startTouchPos; //Position of touch when finger touches the screen
    private int currentPartition = 0;
    private Direction panDirection; //direction of the last finger gesture
    private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
    private bool touchCancelled = false;

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

                    //Cancel touch if finger is touching undesirable objects while panning
                    if(IsTouchingNGUI(startTouchPos) || IsTouchingPet(startTouchPos))
                        touchCancelled = true;
                break;

                case TouchPhase.Moved:
                    if(touchCancelled) return;
                    // Vector2 touchDeltaPosition = touch.deltaPosition;
                    Vector2 nowTouchPos = touch.position;
                    float currentPosX = currentPartition * partitionOffset;

                    //Is touching panning to the left 
                    if(nowTouchPos.x < startTouchPos.x && currentPartition != numOfPartitions){
                        panDirection = Direction.Left;

                        //Get the different of the touch position now and the touch position at start
                        float deltaTouchPosX = Mathf.Abs(nowTouchPos.x - startTouchPos.x);

                        //Divide by screen width to get the normalize position;
                        normalizedTouchPosX = deltaTouchPosX / Screen.width;
                        
                        if(normalizedTouchPosX >= minNormalizedPanDistance){
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = currentPosX + normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
                    }else if(nowTouchPos.x > startTouchPos.x && currentPartition != 0){
                        panDirection = Direction.Right;

                        //Get the different of the touch position now and the touch position at start
                        float deltaTouchPosX = Mathf.Abs(nowTouchPos.x - startTouchPos.x);

                        //Divide by screen width to get the normalize position;
                        normalizedTouchPosX = deltaTouchPosX / Screen.width;

                        if(normalizedTouchPosX >= minNormalizedPanDistance){
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = currentPosX - normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
                    }
                break;

                case TouchPhase.Ended:
                    touchCancelled = false;

                    SnapCameraTo(panDirection);
                break;
            }
        }
    }    

    //************************************************
    // CheckArrowKeys() 
    // Checks arrow key input for moving around the
    // area.  Only use for UNITY_EDITOR
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
    private void MoveCameraWithKey(Direction direction){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBack);
        float moveTo = 0;

        if(direction == Direction.Left)
            currentPartition++;
        else
            currentPartition--;    

        moveTo = partitionOffset * currentPartition;
        LeanTween.moveX(gameObject, moveTo, 0.5f, optional);
        normalizedTouchPosX = 0;
    }

    private void SnapCameraTo(Direction direction){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBack);
        float moveTo = 0;

        //code goes here
        if(normalizedTouchPosX >= 0.5){ //more than half way to the next screen
            if(direction == Direction.Left)
                currentPartition++;
            else
                currentPartition--;    
        }
        moveTo = partitionOffset * currentPartition;

        LeanTween.moveX(gameObject, moveTo, 1.0f, optional);
        normalizedTouchPosX = 0;
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
}