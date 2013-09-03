using UnityEngine;
using System.Collections;

/*
    Attach this script to main camera.
    Panning left or right will move the camera x position right or left, respectively.
    Swiping Left or right will snap the camera x position right or left, respectively.
*/
public class PanToMoveCamera : MonoBehaviour{
    public float minNormalizedPanDistance = 0.05f; //min normalized panning distance
    public int numOfPartitions = 3; //number of partitions allowed
    public float partitionOffset = 80.0f; //How big each partition is in world position

    private Vector2 startTouchPos; //Position of touch when finger touches the screen
    private Vector2 currentTouchPos; //Position of touch right now
    private float startTime; //Time at when finger touches screen
    private int currentPartition = 0;
    private Direction panDirection; //direction of the last finger gesture
    private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
    private bool touchCancelled = false; //True: touch shouldn't be handled
    private float maxSwipeTime = 0.3f; //Swipe gesture needs to be faster than maxSwipeTime

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

                    //Cancel touch if finger is touching undesirable objects while panning
                    if(IsTouchingNGUI(startTouchPos) || IsTouchingPet(startTouchPos))
                        touchCancelled = true;
                break;

                case TouchPhase.Moved:
                    if(touchCancelled) return;
                    // Vector2 touchDeltaPosition = touch.deltaPosition;
                    currentTouchPos = touch.position;
                    float currentPosX = currentPartition * partitionOffset;
                    normalizedTouchPosX = GetNormalizedPosition();

                    //Is touching panning to the left 
                    if(currentTouchPos.x < startTouchPos.x && currentPartition != numOfPartitions){
                        panDirection = Direction.Left;
                        
                        if(normalizedTouchPosX >= minNormalizedPanDistance){
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = currentPosX + normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
                    }else if(currentTouchPos.x > startTouchPos.x && currentPartition != 0){
                        panDirection = Direction.Right;

                        if(normalizedTouchPosX >= minNormalizedPanDistance){
                            //With the normalize position figure out how much the camera will have to move
                            float normalizedPartitionX = normalizedTouchPosX * partitionOffset;
                            float newPosX = currentPosX - normalizedPartitionX;

                            transform.localPosition = new Vector3(newPosX, 0, 0);
                        }
                    }
                break;

                case TouchPhase.Ended:
                    // if(!CheckForSwipeGesture(touch.position))
                    float swipeTime = Time.time - startTime;

                    if(swipeTime <= maxSwipeTime && normalizedTouchPosX >= minNormalizedPanDistance){
                        print("swipe");
                        SwipeSnapCameraTo(panDirection);
                    }else{
                        print("pan snap");
                        PanSnapCameraTo(panDirection);
                    }
                    touchCancelled = false;
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
            SwipeSnapCameraTo( Direction.Left );
        else if ( Input.GetKeyDown( KeyCode.LeftArrow ) )
            SwipeSnapCameraTo( Direction.Right );       
    }

    //Normalized position tells how much of the screen does your gesture cover.
    //From 0 ~ 1
    private float GetNormalizedPosition(){
        //Get the different of the touch position now and the touch position at start
        float deltaTouchPosX = Mathf.Abs(currentTouchPos.x - startTouchPos.x);

        //Divide by screen width to get the normalize position;
        return deltaTouchPosX / Screen.width;
    }

    private void SwipeSnapCameraTo(Direction direction){
        Hashtable optional = new Hashtable();
        float moveTo = 0;

        if(direction == Direction.Left)
            MoveRightOnePartition();
        else
            MoveLeftOnePartition();

        moveTo = partitionOffset * currentPartition;
        LeanTween.moveX(gameObject, moveTo, 0.25f, optional);
        normalizedTouchPosX = 0;
    }

    private void PanSnapCameraTo(Direction direction){
        Hashtable optional = new Hashtable();
        float moveTo = 0;

        if(normalizedTouchPosX >= 0.5){ //more than half way to the next screen
            if(direction == Direction.Left)
                MoveRightOnePartition();
            else
                MoveLeftOnePartition();
        }

        moveTo = partitionOffset * currentPartition;
        LeanTween.moveX(gameObject, moveTo, 0.25f, optional);
        normalizedTouchPosX = 0;
    }

    //Move to the next partition and check boundary
    private void MoveRightOnePartition(){
        if(currentPartition + 1 <= numOfPartitions)
            currentPartition++;
    }

    //Move to previous partition and check boundary
    private void MoveLeftOnePartition(){
        if(currentPartition - 1 >= 0)
            currentPartition--;
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