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

    private const int mFramesUntilActivSet = 3;
    private int mFramesSinceInactiveSet = 0;

	private float mInvinciblePulse = 0f;
	private float mSpeedBoostPulse = 0f; // Boosts from items
	private float mSpeedIncreasePulse = 0f; // Time til we speed up our constant speed
    private float mSpeed;
    private float mCurrentTimeMultiplier;
	private float mSpeedBoostAmmount = 0f;
    private bool mbInvincible;
    private bool mbJumping;
    private bool mbGrounded;
    private bool mbFalling;
    private bool mbTriggerColliding;
    private Vector2 mInitialPosition;
    private Vector3 mMovementVector;
    private Vector3 mLastPosition;
	private CharacterController mCharacterController;
    private List<Collider> mIgnoringCollisions = new List<Collider>();

    public bool Invincible { get { return mbInvincible; } }
    public float Speed { get { return mSpeed; } }

	// Use this for initialization
	void Start() {
        mInitialPosition = transform.position;

        mCharacterController = gameObject.GetComponent<CharacterController>();
        if (mCharacterController == null)
            Debug.LogError("Character Controller not attached!");

        // Slightly redundant in some ways, but keeps some logic together.
        Reset();
	}

	// Update is called once per frame
	void Update() {
        if (mbFalling) {
            mFramesSinceInactiveSet++;
            if (mFramesSinceInactiveSet >= mFramesUntilActivSet) {
                mbFalling = false;
            }
        }
        // Unignore all current collisions
		UpdateInput();

        UpdateInvincible();
	}
	
    // The more physics-y update. Called multiple times per frame.
	void FixedUpdate() {
		UpdateSpeed();

		UpdateMovement();

		UpdateFalling();

		if (!Vector3.Equals(mLastPosition, transform.position))
			mLastPosition = transform.position;

		CheckAndActOnDeath();
	}
	
	void onSwipeUp() {
		TriggerJump();
	}

	void onSwipeDown() {
		TriggerFall();
	}

    void onPlayerJumpBegin() {}
    void onPlayerJumpEnd() {}
    void onPlayerFallBegin() {}
    void onPlayerFallEnd() {}

    public void Reset() {
        mCurrentTimeMultiplier = 1f;
        // Go back to the original position.
		transform.position = mInitialPosition;
        // Reset some timers
        mSpeedIncreasePulse = SpeedIncreaseTime;
        mSpeedBoostPulse = 0f;
        mInvinciblePulse = 0f;
        // Player values
        mLastPosition = transform.position;
        mMovementVector = Vector3.zero;
        mLastPosition = Vector3.zero;
        mSpeed = DefaultSpeed;
        mbInvincible = false;
        // Collision values
        mbTriggerColliding = false;
        mbGrounded = false;
        mbFalling = false;
        mbJumping = false;
        //@TODO you may have to reset all ignoring collisions before clearing. idk.
        mIgnoringCollisions.Clear();

        RunnerAnimationController animationController = GetComponent<RunnerAnimationController>();
        if (animationController != null)
            animationController.Reset();
}

    private void UpdateInvincible() {
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

                        // Reset movement vector
                        mMovementVector = Vector3.zero;
                    } else {
                        bool bSomethingBelow = Physics.Raycast(transform.position, Vector3.down, out hitInfo);
                        if (bSomethingBelow)
                            mbInvincible = false;
                    }
                }
            }
        }
    }

    private void UpdateSpeed() {
        mSpeedIncreasePulse -= Time.deltaTime;
        if (mSpeedIncreasePulse <= 0) {
            //mSpeed += SpeedIncrease;
            mCurrentTimeMultiplier += SpeedIncrease;
            RunnerGameManager.GetInstance().IncreaseTimeSpeed(SpeedIncrease);
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
    private void UpdateFalling(Collider inCollider = null) {
        if (mbFalling) {
            if (!mbTriggerColliding || mbGrounded) {
                mbFalling = false;
                BroadcastMessage("onPlayerFallEnd", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void UpdateMovement() {
        // These are constant speeds, not forces. It's weird I know.
        //float currentSpeed = mSpeed + mSpeedBoostAmmount;
        float currentDeltaTime = Time.deltaTime;

        mMovementVector.z = mSpeed + mSpeedBoostAmmount;

        // Add in Gravity force.
        mMovementVector += (Physics.gravity * rigidbody.mass) * currentDeltaTime;

        // Vertical drag
        mMovementVector += (Vector3.down * rigidbody.drag * currentDeltaTime);

        if (mCharacterController == null)
            Debug.LogError("No Character Controller exists!");

        // Perform the move
        CollisionFlags flags = mCharacterController.Move(mMovementVector * currentDeltaTime);
        bool isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
        if (isGrounded && mbJumping) {
            mbJumping = false;
            BroadcastMessage("onPlayerJumpEnd", SendMessageOptions.DontRequireReceiver);
        }

        // Reset movement.
        if (isGrounded) {
            mMovementVector = new Vector3();

            if (!mbGrounded) {
                // Was previously in freefall.
                for (int ignoredIndex = mIgnoringCollisions.Count - 1; ignoredIndex >= 0; ignoredIndex--) {
                    Collider ignoredCollider = mIgnoringCollisions[ignoredIndex];
                    if (ignoredCollider != null)
                        Physics.IgnoreCollision(ignoredCollider, collider, false);
                    else
                        mIgnoringCollisions.RemoveAt(ignoredIndex);
                }
            }
        }

        mbGrounded = isGrounded;

        Vector3 position = transform.position;
        position.x = mInitialPosition.x;
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

    public void TriggerInvincibility(float inDuration) {
        mInvinciblePulse = inDuration;
        mbInvincible = true;
    }

    public void TriggerSpeedBoost(float inDuration, float inSpeedAmmount) {
        mSpeedBoostAmmount = inSpeedAmmount;
        mSpeedBoostPulse = inDuration;
    }

	private void CheckAndActOnDeath() {
		// Are we below the maps floor value
		LevelManager levelManager = RunnerGameManager.GetInstance().LevelManager;
        float yTooLowValue = levelManager.GetTooLowYValue(transform.position);
        if (transform.position.y < yTooLowValue) {
            if (!mbInvincible)
                RunnerGameManager.GetInstance().ActivateGameOver();
            else {
                Vector3 position = transform.position;
                position.y = yTooLowValue;
                transform.position = position;
            }
		}
	}

	public void TriggerSlowdown(float inDivisor) {
        if (!mbInvincible) { 
            mSpeed /= inDivisor;

            if (mSpeed < DefaultSpeed)
                mSpeed = DefaultSpeed;

            MegaHazard megaHazard = GameObject.FindGameObjectWithTag("MegaHazard").GetComponent<MegaHazard>();
            megaHazard.TriggerPlayerSlowdown();
        }
	}

	private void TriggerJump() {
		if (mbGrounded && !mbJumping && !mbFalling) {
			mMovementVector.y += JumpSpeed;
            mbJumping = true;
            BroadcastMessage("onPlayerJumpBegin", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void TriggerFall() {
		if (!mbJumping && !mbFalling) {
            // nop you can't fall through a bottom layered collision.
            int bottomLayer = RunnerGameManager.GetInstance().LevelManager.BottomLayer;

            RaycastHit hitinfo;
            if (Physics.Raycast(collider.bounds.min, Vector3.down, out hitinfo, 1f)) {
                Collider hitCollider = hitinfo.collider;
                if (hitCollider.gameObject.layer != bottomLayer) {
                    Physics.IgnoreCollision(hitinfo.collider, collider);
                    mIgnoringCollisions.Add(hitinfo.collider);
                    mbFalling = true;
                    mbGrounded = false;
            		BroadcastMessage("onPlayerFallBegin", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
	}
}
