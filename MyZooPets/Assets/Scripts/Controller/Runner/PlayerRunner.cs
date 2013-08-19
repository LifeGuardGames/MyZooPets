using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerRunner : MonoBehaviour
{
    public float DefaultSpeed = 1.0f;
	public float JumpSpeed = 0.5f;
	public float SpeedIncrease = 0.1f;
	public float SpeedIncreaseTime = 5;

	private float mInvinciblePulse = 0f;
	private float mSpeedBoostPulse = 0f; // Boosts from items
	private float mSpeedIncreasePulse = 0f; // Time til we speed up our constant speed

    private float mInitialXPos = 0;
    private float mSpeed = 0.1f;
	private float mDistanceTravelled = 0f;
	private float mSpeedBoostAmmount = 0f;
	private bool mbInvincible = false;
	private bool mbJumping = false;
	private bool mbColliding = false;
	private bool mbGrounded = false;
	private bool mbFalling = false;
	private bool mbTriggerColliding = false;
	private Vector3 mMovementVector = Vector3.zero;
	private Vector3 mLastPosition = Vector3.zero;
	private CapsuleCollider mCapsuleTrigger;
	private CharacterController mCharacterController;

    public bool Invincible { get { return mbInvincible; } }
    public float Speed { get { return mSpeed; } }

	// Use this for initialization
	void Start() {
        mInitialXPos = transform.position.x;

		mCharacterController = gameObject.GetComponent<CharacterController>();
		Transform layerObject = transform.FindChild("LayerTrigger");
		if (layerObject != null) {
			mCapsuleTrigger = layerObject.GetComponent<CapsuleCollider>();
			Physics.IgnoreCollision(mCharacterController, mCapsuleTrigger);
		} else
			Debug.LogError("The player requires a capsule collider trigger child called 'LayerTrigger'!!!!");

		if (mCharacterController == null)
			Debug.LogError("Character Controller not attached!");
		mSpeedIncreasePulse = SpeedIncreaseTime;
		mLastPosition = transform.position;
        mSpeed = DefaultSpeed;
	}

	// Update is called once per frame
	void Update() {
		UpdateInput();

		if (mbInvincible) {
			mInvinciblePulse -= Time.deltaTime;
			if (mInvinciblePulse <= 0f) {
				// Ready to uninvincible, except, we need to make sure we don't do it
				//while the character is dying...
                if (mbGrounded)
                    mbInvincible = false;
                else {
				    // Raycast around up. If there is something to latch on to, go to it!
                    RaycastHit hitInfo;
                    bool bSomethingAbove = Physics.Raycast(transform.position, Vector3.up, out hitInfo);
                    if (bSomethingAbove) {
                        // Latch onto it
                        Vector3 newPosition = hitInfo.transform.position;
                        newPosition.y += mCharacterController.height / 2;
                        transform.position = newPosition;
                    } else {
                        bool bSomethingBelow = Physics.Raycast(transform.position, Vector3.down, out hitInfo);
                        if (bSomethingBelow)
                            mbInvincible = false;
                    }
                }
			}
		}
	}
	
	void FixedUpdate() {
		UpdateSpeed();

		UpdateMovement();
		
		UpdateFalling();

		if (!Vector3.Equals(mLastPosition, transform.position))
			mLastPosition = transform.position;

		CheckAndActOnDeath();
	}
	
	void OnControllerColliderHit(ControllerColliderHit inHit) {
	}
	
	void onSwipeUp() {
		TriggerJump();
	}

	void onSwipeDown() {
		TriggerFall();
	}

	void LayerTriggerCollisionEnter(Collider inCollider) {
		mbTriggerColliding = true;
	}

	void LayerTriggerCollisionStay(Collider inCollider) {
		mbTriggerColliding = true;
	}

	void LayerTriggerCollisionExit(Collider inCollider) {
		mbTriggerColliding = false;
	}

	private void CheckAndActOnDeath() {
		RunnerGameManager gameManager = RunnerGameManager.GetInstance();

		// Are we below the maps floor value
		LevelManager levelManager = gameManager.LevelManager;
		if (transform.position.y < levelManager.LevelTooLowYValue) {
            if (!mbInvincible)
                gameManager.ActivateGameOver();
            else {
                Vector3 position = transform.position;
                position.y = levelManager.LevelTooLowYValue;
                transform.position = position;
            }
		}
	}

	public void TriggerSlowdown(float inDivisor) {
        if (!mbInvincible) { 
            mSpeed /= inDivisor;

            if (mSpeed < DefaultSpeed)
                mSpeed = DefaultSpeed;
        }
	}

	private void UpdateSpeed() {
		mSpeedIncreasePulse -= Time.deltaTime;
		if (mSpeedIncreasePulse <= 0) {
            mSpeed += SpeedIncrease;
			mSpeedIncreasePulse = SpeedIncreaseTime;
		}

		if (mSpeedBoostPulse > 0f) {
			mSpeedBoostPulse -= Time.deltaTime;
			if (mSpeedBoostPulse <= 0f) {
				mSpeedBoostPulse = 0f;
				mSpeedBoostAmmount = 0f;
			}
		}
	}

	// Checks if we are "falling down" to re-eneable collision.
	private void UpdateFalling() {
		if (mbFalling) {
			if (!mbTriggerColliding || mbGrounded)
				mbFalling = false;
		}

		if (gameObject.layer != 0) {
			if (mbJumping) {
				// If we begin falling downward, reset the layer
				Vector3 currentMovementDirection = mLastPosition - transform.position;
				if (currentMovementDirection.y > 0) {
					gameObject.layer = 0;
				}
			} else if (!mbFalling) {
				gameObject.layer = 0;
			}
		}

	}

	private void UpdateMovement() {
		// These are constant speeds, not forces. It's weird I know.
        mMovementVector.z = mSpeed + mSpeedBoostAmmount;

		// Add in Gravity force.
		mMovementVector += (Physics.gravity * rigidbody.mass) * Time.deltaTime;

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

        Vector3 position = transform.position;
        position.x = mInitialXPos;
        transform.position = position;
	}

	private void UpdateInput() {
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			// Add in jump, since we are grounded, if its pressed.
			TriggerJump();
		} else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
			// Add in jump, since we are grounded, if its pressed.
			TriggerFall();
		}
	}

	private void TriggerJump() {
		if (mbGrounded && !mbJumping) {
			mMovementVector.y += JumpSpeed;
			gameObject.layer = 12;
			mbJumping = true;
		}
	}

	private void TriggerFall() {
		if (!mbFalling) {
			mbFalling = true;
			gameObject.layer = 12;
		}
	}

	public void TriggerInvincibility(float inDuration) {
		mInvinciblePulse = inDuration;
		mbInvincible = true;
	}

	public void TriggerSpeedBoost(float inDuration, float inSpeedAmmount) {
		mSpeedBoostAmmount = inSpeedAmmount;
		mSpeedBoostPulse = inDuration;
	}
}
