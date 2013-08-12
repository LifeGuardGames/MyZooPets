using UnityEngine;
using System.Collections;

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
	private Vector3 mMovementVector = Vector3.zero;
	private CharacterController mCharacterController;
	
	// Use this for initialization
	void Start() {
		mCharacterController = gameObject.GetComponent<CharacterController>();
		if (mCharacterController == null)
			Debug.LogError("Character Controller not attached!");
		mSpeedIncreasePulse = SpeedIncreaseTime;
    }

    // Update is called once per frame
    void Update() {
        if (mCharacterController == null)
            mCharacterController = GetComponent<CharacterController>();

        UpdateMovement();

        UpdateSpeed();
        CheckAndActOnDeath();
    }

    void OnCollisionEnter(Collision inOther) {
        mbColliding = true;
        mbJumping = false;
    }

    void OnCollisionExit(Collision inOther) {
        mbColliding = false;
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

    private void UpdateMovement()
    {
        if (mbGrounded)
        {
            // Reset movement.
            mMovementVector = new Vector3();
            // These are constant speeds, not forces. It's wierd I know.
            mMovementVector.z = Speed;

            // Add in jump, since we are grounded, if its pressed.
            if (Input.GetKeyDown("space"))
            {
                mMovementVector.y = JumpSpeed;
            }
        }

        // Add in Gravity force.
        mMovementVector += Physics.gravity * Time.deltaTime;

        if (mCharacterController == null)
            Debug.LogError("No Character Controller exists!");
        CollisionFlags flags = mCharacterController.Move(mMovementVector * Time.deltaTime);
        mbGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }
}
