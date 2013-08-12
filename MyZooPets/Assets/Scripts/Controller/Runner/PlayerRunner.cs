using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerRunner : MonoBehaviour
{
	public float Speed = 0.1f;
	public float JumpSpeed = 0.5f;
	public float Mass = 1.0f;
	public float SpeedIncrease = 0.1f;
	public float SpeedIncreaseTime = 5;

	private float mSpeedIncreasePulse = 0.0f;
	private float mDistanceTravelled = 0.0f;
	private bool mbColliding = false;
	private bool mbJumping = false;
	private bool mbGrounded = false;
    private bool mbFalling = false;
    private Vector3 mMovementVector = Vector3.zero;
    private Vector3 mLastPosition = Vector3.zero;
    private CharacterController mCharacterController;
	private List<Collider> mCurrentCollisions = new List<Collider>();
	private List<Collider> mIgnoringCollisions = new List<Collider>();
	
	// Use this for initialization
	void Start() {
		mCharacterController = gameObject.GetComponent<CharacterController>();
		if (mCharacterController == null)
			Debug.LogError("Character Controller not attached!");
		mSpeedIncreasePulse = SpeedIncreaseTime;
        mLastPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        UpdateInput();
    }
	
	void FixedUpdate() {
		
        UpdateSpeed();


		mCurrentCollisions.Clear();
        UpdateMovement();
        
        UpdateFalling();

        if (!Vector3.Equals(mLastPosition, transform.position))
            mLastPosition = transform.position;

        CheckAndActOnDeath();
	}
	
	void OnControllerColliderHit(ControllerColliderHit inHit)
	{
		if (!mCurrentCollisions.Contains(inHit.collider))
			mCurrentCollisions.Add(inHit.collider);
		
		if (mIgnoringCollisions.Count > 0) {
			foreach (Collider ignored in mIgnoringCollisions)
			{
				Physics.IgnoreCollision(mCharacterController, ignored, false);
			}
			mIgnoringCollisions.Clear ();
		}
	}
	
    void onSwipeUp() {
        // Attempt to move to a platform above
        Debug.Log("Going Up");
    }

    void onSwipeDown() {
        // Attempt to move to a platform below
        Debug.Log("Going Down");
    }

    public void TriggerSlowdown(float inDivisor)
    {
        Speed /= inDivisor;
    }

    private void CheckAndActOnDeath()
    {
        RunnerGameManager gameManager = ((GameObject)GameObject.FindGameObjectWithTag("GameManager")).GetComponent<RunnerGameManager>();

        // Are we below the maps floor value
        LevelManager levelManager = ((GameObject)GameObject.FindGameObjectWithTag("LevelManager")).GetComponent<LevelManager>();
        if (transform.position.y < levelManager.LevelTooLowYValue)
        {
            gameManager.ActivateGameOver();
        }
    }

    private void UpdateSpeed()
    {
        mSpeedIncreasePulse -= Time.deltaTime;
        if (mSpeedIncreasePulse <= 0)
        {
            Speed += SpeedIncrease;
            mSpeedIncreasePulse = SpeedIncreaseTime;
        }
    }

    // I can't name things.
    // Checks if we are "falling down" to re-eneable collision.
    // Assuming it wasn't enabled already.
    private void UpdateFalling()
    {
        if (gameObject.layer != 0) {
            if (mbJumping) {
                Vector3 currentMovementDirection = mLastPosition - transform.position;
                if (currentMovementDirection.y > 0) {
                    gameObject.layer = 0;
                }
            } else if (!mbFalling) {
                gameObject.layer = 0;
            }
        }

    }

    private void UpdateMovement()
    {
        if (mbGrounded) {
            // These are constant speeds, not forces. It's wierd I know.
            mMovementVector.z = Speed;
        }

        // Add in Gravity force.
        mMovementVector += Physics.gravity * Time.deltaTime;

        if (mCharacterController == null)
            Debug.LogError("No Character Controller exists!");

        CollisionFlags flags = mCharacterController.Move(mMovementVector * Time.deltaTime);
        bool isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
        if (isGrounded && mbJumping)
            mbJumping = false;
		
        // Reset movement.
        if (isGrounded)
            mMovementVector = new Vector3();
		

        mbGrounded = isGrounded;
    }

    private void UpdateInput()
    {
        // Add in jump, since we are grounded, if its pressed.
        if (!mbJumping && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            TriggerJump();
        }
        // Add in jump, since we are grounded, if its pressed.
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            TriggerFall();
        }
    }

    private void TriggerJump()
    {
        mMovementVector.y += JumpSpeed;
        gameObject.layer = 12;
		mbJumping = true;
    }

    private void TriggerFall()
    {
		mbFalling = true;
		
		foreach (Collider currentCollision in mCurrentCollisions)
		{
			if (!mIgnoringCollisions.Contains(currentCollision))
			{
				mIgnoringCollisions.Add(currentCollision);
				Physics.IgnoreCollision(mCharacterController, currentCollision);
			}
		}
		
    }
}
