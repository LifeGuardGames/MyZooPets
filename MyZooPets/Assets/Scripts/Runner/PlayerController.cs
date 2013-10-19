using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Singleton<PlayerController> {
    
    // Player Handling
    public float gravity = 20;
    public float speed = 8;
    public float acceleration = 30;
    public float jumpHeight = 12;
    
    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove;
    
    private PlayerPhysics playerPhysics;
    private int numOfLayerToPassThrough = 0;

    void Start () {
        playerPhysics = GetComponent<PlayerPhysics>();
    }
    
    void Update () {
        targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
        // targetSpeed = speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration);

#if UNITY_EDITOR
        if(Input.GetKey("up")) Jump();
        if(Input.GetKey("down")) Drop();
#endif

        if(playerPhysics.Grounded && !playerPhysics.Jumping)
            amountToMove.y = 0;
        
        amountToMove.x = currentSpeed;
        amountToMove.y -= gravity * Time.deltaTime;
        playerPhysics.Move(amountToMove * Time.deltaTime, ref numOfLayerToPassThrough);
    }

    void OnTap(TapGesture gesture) { 
        print("tap");
        Jump();
    }

    void OnSwipe(SwipeGesture gesture) { 
        print("swipe");
        FingerGestures.SwipeDirection direction = gesture.Direction;
        if(direction == FingerGestures.SwipeDirection.Down){
            Drop();
        }
    } 

    private void Jump(){
        if(playerPhysics.Grounded){
            amountToMove.y = jumpHeight;
            playerPhysics.Jumping = true;
        }
    }

    private void Drop(){
        if(playerPhysics.Grounded && !playerPhysics.Jumping)
            // amountToMove.y = 0;
            numOfLayerToPassThrough = 1;
    }
}
