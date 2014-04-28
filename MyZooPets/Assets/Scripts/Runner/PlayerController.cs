using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Singleton<PlayerController> {
    public static EventHandler<EventArgs> OnJump;
    public static EventHandler<EventArgs> OnDrop;
	
    // public float speedIncreaseTime = 5; //How long before the next game speed increment 

    [System.Serializable]
    public class PlatformerControllerMovement{
        
		public float defaultTargetSpeed = 15f; //The default running speed
        [System.NonSerialized]
		public float currentSpeed = 0f; //currvent movement speed after it gets smoothed by acceleration
		public float targetSpeed; //The speed you want the character to reach to
		public float acceleration = 5f; //How fast does the character change speed? higher is faster
		public float jumpHeight = 9;
		[System.NonSerialized]
		public float verticalSpeed = 0f;
		public float maxFallSpeed = 100f; //maximum speed the player is allowed to fall

        private float gravity = 130f; //gravity is calculated based on the target speed
       
		/// <summary>
		/// Gets the gravity. Dependent on the target speed
		/// </summary>
		/// <value>The gravity.</value>
		public float Gravity{
			get{
				return this.targetSpeed * 10;
			}
		}

		/// <summary>
		/// Increases the target speed.
		/// </summary>
		/// <param name="isNormal">If set to <c>true</c> is normal.</param>
		public void IncreaseTargetSpeed(bool isNormal){
			if(isNormal)
				this.targetSpeed += 5; //speed increases by an increment of 5
			else
				this.targetSpeed += 10;

			//cap the speed at 45
			if(this.targetSpeed >= 45)
				this.targetSpeed = 45;
		}

		/// <summary>
		/// Resets the target speed.
		/// </summary>
        public void ResetTargetSpeed(){
            targetSpeed = defaultTargetSpeed; 
        }
    }

    public PlatformerControllerMovement movement = new PlatformerControllerMovement();
	public float timeUntilTargetSpeedIncrease = 30f;

    private Vector2 amountToMove; //How much you want the player to move
    private PlayerPhysics playerPhysics; //Reference to physics
    private float speedIncreaseCounter = 0f; //Time till we speed up the game
    private Vector2 initialPosition; //Where the player start
	private GameObject floatyLocation;

#if UNITY_EDITOR	
	// used just for testing keyboard input in unity editor
	private bool bDelay = false;
#endif

	public GameObject FloatyLocation{
		get{
			return floatyLocation;
		}
	}
	
    void Start () {
        playerPhysics = GetComponent<PlayerPhysics>();
        initialPosition = this.transform.position;
		floatyLocation = this.transform.Find("FloatyLocation").gameObject;
        Reset();
    }
    
    void Update () {
        if(!RunnerGameManager.Instance.GameRunning)
            return;

#if UNITY_EDITOR    
        CheckKeyMovement();
#endif

		UpdateSpeed();
        CheckAndActOnDeath();
    }
	
	void OnGUI(){
		 if(GUI.Button(new Rect(0, 0, 100, 100), "+speed")){
			movement.targetSpeed += 5;
		 }

		if(GUI.Button(new Rect(100, 0, 100, 100), "-speed")){
			movement.targetSpeed -= 5;
		}

		GUI.Label(new Rect(200, 0, 100, 100), movement.Gravity.ToString());
	}

    void FixedUpdate(){
        if(!RunnerGameManager.Instance.GameRunning)
            return;

		//update runner horizontal movement
        UpdateHorizontalMovement();

		//apply game gravity
        ApplyGravity(); 

        amountToMove = new Vector2(movement.currentSpeed, movement.verticalSpeed);

        //always want movement to be framerate independent
        amountToMove *= Time.deltaTime;

        playerPhysics.Move(amountToMove);
    }

	/// <summary>
	/// Reset player position and physics
	/// </summary>
    public void Reset(){
        // speedIncreaseCounter = speedIncreaseTime;
        transform.position = initialPosition;

        movement.verticalSpeed = 0f;
        movement.currentSpeed = 0f;
        movement.ResetTargetSpeed();
    }

    private void UpdateHorizontalMovement(){
        movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, movement.targetSpeed, 
            movement.acceleration * Time.deltaTime); 
    }

	/// <summary>
	/// Applies the gravity.
	/// gravity will be changed manually depending on the speed of the horizontal
	/// movement
	/// </summary>
    private void ApplyGravity(){

        //if grounded just set speed to gravity speed
        if(playerPhysics.Grounded && !playerPhysics.Jumping){
            movement.verticalSpeed = -movement.Gravity * Time.deltaTime;
        }
        //if jumping keep decreasing the vertical speed by gravity
        else{
            movement.verticalSpeed -= movement.Gravity * Time.deltaTime;
        }

        //make sure we don't fall nay faster than maxFallSpeed
        movement.verticalSpeed = Mathf.Max(movement.verticalSpeed, -movement.maxFallSpeed);
    }

	/// <summary>
	/// Calculates the jump vertical speed.
	/// </summary>
	/// <returns>The vertical speed.</returns>
	/// <param name="targetJumpHeight">Target jump height.</param>
    private float CalculateJumpVerticalSpeed(float targetJumpHeight){
        // from jump height and gravity we deduce the upwards speed for character
        // at apex
        return Mathf.Sqrt(2 * targetJumpHeight * movement.Gravity);
    }

    private void Jump(){
        if(playerPhysics.Grounded){
//            AudioManager.Instance.PlayClip( "runnerJumpUp" );

            // amountToMove.y = jumpHeight;
            movement.verticalSpeed = CalculateJumpVerticalSpeed(movement.jumpHeight);

            playerPhysics.Jumping = true;
        }
    }  

    private void Drop(){
        if(playerPhysics.Grounded && !playerPhysics.Jumping && !playerPhysics.Falling)
            AudioManager.Instance.PlayClip( "runnerJumpDown" );

        playerPhysics.AllowPassThroughLayer = true;
    }
   

#if UNITY_EDITOR
     //---------------------------------------------------
    // UpdateMovement()
    // Moves the player along the x axis with default speed. 
    // Check for jumping and falling physics as well.
    //---------------------------------------------------
    private void CheckKeyMovement(){

        if(Input.GetKey("up")) Jump();
        if(Input.GetKey("down") && !playerPhysics.Falling && !bDelay) {
            bDelay = true;
            Drop();
        }
        else
            bDelay = false;
    }
#endif

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
    // TriggerSlowdown()
    // Slow down the game and decrease the distance between 
    // player and megahazard
    //---------------------------------------------------
    public void TriggerSlowdown(float inDivisor) {
//        RunnerGameManager.Instance.SlowTimeSpeed(inDivisor);
		movement.ResetTargetSpeed();
        MegaHazard.Instance.TriggerPlayerSlowdown();

    }
	
    //---------------------------------------------------
    // UpdateSpeed()
    // Increase the pace of the game
    //---------------------------------------------------
    private void UpdateSpeed(){
		speedIncreaseCounter += Time.deltaTime;
		if(speedIncreaseCounter >= timeUntilTargetSpeedIncrease){
			//increase time
			movement.IncreaseTargetSpeed(true);
			speedIncreaseCounter = 0;
		}
    }


    //---------------------------------------------------
    // CheckAndActOnDeath()
    // If player falls below the "Dead line" than the player dies
    //---------------------------------------------------
    private void CheckAndActOnDeath(){
       RunnerLevelManager runnerLevelManager = RunnerLevelManager.Instance;
        if(transform.position.y < runnerLevelManager.LevelTooLowYValueGameOver){
            AudioManager.Instance.PlayClip("runnerDie");
            RunnerGameManager.Instance.ActivateGameOver();
        } 
    }
}
