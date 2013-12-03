using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Singleton<PlayerController> {
    public static EventHandler<EventArgs> OnJump;
    public static EventHandler<EventArgs> OnDrop;

    public float speedIncrease = 0.1f; //The amount the game will be speed up by
    public float speedIncreaseTime = 5; //How long before the next game speed increment 

    // Player Handling
    public float gravity = 20;
    public float defaultSpeed = 8;
    public float acceleration = 30;
    public float jumpHeight = 12;
    
    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove; //How much you want the player to move
    private PlayerPhysics playerPhysics; //Reference to physics
    private float speedIncreaseCounter = 0f; //Time till we speed up the game
    private Vector2 initialPosition; //Where the player start

    void Start () {
        playerPhysics = GetComponent<PlayerPhysics>();
        initialPosition = transform.position;

        Reset();
    }
    
    void Update () {
        if(!RunnerGameManager.Instance.GameRunning)
            return;

        UpdateSpeed();
        CheckAndActOnDeath();
    }

    void FixedUpdate(){
        if(!RunnerGameManager.Instance.GameRunning)
            return;

        targetSpeed = defaultSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration);

        UpdateMovement();
    }

    //Listen to tap gesture from finger gesture plugin
    void OnTap(TapGesture gesture) { 
        if(RunnerGameManager.Instance.GameRunning){
            Jump();
            
            if(OnJump != null)
                OnJump(this, EventArgs.Empty);
        }
    }

    //Listen to swipe down gesture
    void OnSwipe(SwipeGesture gesture) { 
        FingerGestures.SwipeDirection direction = gesture.Direction;
        if(direction == FingerGestures.SwipeDirection.Down){
            Drop();

            if(OnDrop != null)
                OnDrop(this, EventArgs.Empty);
        }
    } 

    //---------------------------------------------------
    // Reset()
    // Setup player for a new game
    //---------------------------------------------------
    public void Reset(){
        speedIncreaseCounter = speedIncreaseTime;
        transform.position = initialPosition;
    }

    //---------------------------------------------------
    // TriggerSlowdown()
    // Slow down the game and decrease the distance between 
    // player and megahazard
    //---------------------------------------------------
    public void TriggerSlowdown(float inDivisor) {
        RunnerGameManager.Instance.SlowTimeSpeed(inDivisor);
        MegaHazard.Instance.TriggerPlayerSlowdown();
    }

    //---------------------------------------------------
    // UpdateMovement()
    // Moves the player along the x axis with default speed. 
    // Check for jumping and falling physics as well.
    //---------------------------------------------------
    private void UpdateMovement(){

#if UNITY_EDITOR
        if(Input.GetKey("up")) Jump();
        if(Input.GetKey("down") && !playerPhysics.Falling) Drop();
#endif

        if(playerPhysics.Grounded && !playerPhysics.Jumping)
            amountToMove.y = 0;
        
        amountToMove.x = currentSpeed;
        amountToMove.y -= gravity * Time.deltaTime;
        playerPhysics.Move(amountToMove * Time.deltaTime);
    }

    //---------------------------------------------------
    // UpdateSpeed()
    // Increase the pace of the game
    //---------------------------------------------------
    private void UpdateSpeed(){
        speedIncreaseCounter -= Time.deltaTime / Time.timeScale;
        if(speedIncreaseCounter <= 0){
            RunnerGameManager.Instance.IncreaseTimeSpeed(speedIncrease);
            speedIncreaseCounter = speedIncreaseTime;
        }
    }

    //---------------------------------------------------
    // CheckAndActOnDeath()
    // If player falls below the "Dead line" than the player dies
    //---------------------------------------------------
    private void CheckAndActOnDeath(){
       LevelManager levelManager = LevelManager.Instance;
        if(transform.position.y < levelManager.LevelTooLowYValueGameOver){
            AudioManager.Instance.PlayClip("runnerDie");
            RunnerGameManager.Instance.ActivateGameOver();
        } 
    }

    private void Jump(){
        if(playerPhysics.Grounded){
            AudioManager.Instance.PlayClip( "runnerJumpUp" );

            amountToMove.y = jumpHeight;
            playerPhysics.Jumping = true;
        }
    }

    private void Drop(){
        if(playerPhysics.Grounded && !playerPhysics.Jumping && !playerPhysics.Falling)
            AudioManager.Instance.PlayClip( "runnerJumpDown" );

            playerPhysics.AllowPassThroughLayer = true;
    }
}
