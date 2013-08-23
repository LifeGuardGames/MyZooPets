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
    public int DefaultLayer = 0;
    public int PassLayer = 30;

	private float mInvinciblePulse = 0f;
	private float mSpeedBoostPulse = 0f; // Boosts from items
	private float mSpeedIncreasePulse = 0f; // Time til we speed up our constant speed
    private float mSpeed;
	private float mSpeedBoostAmmount = 0f;
    private bool mbInvincible;
    private bool mbJumping;
    private bool mbColliding;
    private bool mbGrounded;
    private bool mbFalling;
    private bool mbTriggerColliding;
    private Vector2 mInitialPosition;
    private Vector3 mMovementVector;
    private Vector3 mLastPosition;
    private PlayerLayerTrigger mPlayerTrigger;
	private CharacterController mCharacterController;
    private List<Collider> mGroundedCollision = new List<Collider>();

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

	void LayerTriggerCollisionEnter(Collider inCollider) {
        int bottomLayer = RunnerGameManager.GetInstance().LevelManager.BottomLayer;
        if (inCollider.gameObject.layer != bottomLayer)
		    mbTriggerColliding = true;
        UpdateFalling(inCollider);
	}

	void LayerTriggerCollisionStay(Collider inCollider) {
        mbTriggerColliding = true;
        UpdateFalling(inCollider);
	}

	void LayerTriggerCollisionExit(Collider inCollider) {
        mbTriggerColliding = false;
        UpdateFalling(inCollider);
	}

    public void Reset() {
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
        mbColliding = false;
        mbTriggerColliding = false;
        mbGrounded = false;
        mbFalling = false;
        mbJumping = false;
        gameObject.layer = 0;
        mGroundedCollision.Clear();

        Transform layerObject = transform.FindChild("LayerTrigger");
        if (layerObject != null) {
            mPlayerTrigger = layerObject.GetComponent<PlayerLayerTrigger>();
            Collider layerCollider = layerObject.GetComponent<Collider>();
            Physics.IgnoreCollision(mCharacterController, layerCollider);
        } else
            Debug.LogError("The player requires a capsule collider trigger child called 'LayerTrigger'!!!!");

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
    private void UpdateFalling(Collider inCollider = null) {
        if (mbFalling) {
            if (!mbTriggerColliding || mbGrounded) {
                mbFalling = false;
                BroadcastMessage("onPlayerFallEnd", SendMessageOptions.DontRequireReceiver);
            }
        }

        if (gameObject.layer != 0) {
            if (mbJumping) {
                // Only update this when we are given a collider to check.
                if (inCollider != null && !mGroundedCollision.Contains(inCollider)) {
                    // If we are given a collider, and if we arent touching a collider that we were touching when grounded...
                    // Determine the lowest point of us
                    Vector3 playerTriggerMin = mPlayerTrigger.collider.bounds.min;
                    // the top of them
                    Vector3 colliderMax = inCollider.bounds.max;
                    // Distance between
                    float yDistanceBetween = Mathf.Abs(colliderMax.y - playerTriggerMin.y);
                    // And how much we are willing to forgive for being off 0
                    const float yDistanceForgiveness = 0.5f;
                    if (yDistanceBetween <= yDistanceForgiveness) {
                        gameObject.layer = 0;
                    }
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

        // Vertical drag
        mMovementVector += (Vector3.down * rigidbody.drag * Time.deltaTime);

        if (mCharacterController == null)
            Debug.LogError("No Character Controller exists!");

        // Perform the move
        CollisionFlags flags = mCharacterController.Move(mMovementVector * Time.deltaTime);
        bool isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
        if (isGrounded && mbJumping) {
            mbJumping = false;
            BroadcastMessage("onPlayerJumpEnd", SendMessageOptions.DontRequireReceiver);
        }

        // Reset movement.
        if (isGrounded)
            mMovementVector = new Vector3();

        if (mbGrounded) {
            mGroundedCollision = new List<Collider>(mPlayerTrigger.CurrentColliders);
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
		RunnerGameManager gameManager = RunnerGameManager.GetInstance();

		// Are we below the maps floor value
		LevelManager levelManager = gameManager.LevelManager;
        float yTooLowValue = levelManager.GetTooLowYValue(transform.position);
        if (transform.position.y < yTooLowValue) {
            if (!mbInvincible)
                gameManager.ActivateGameOver();
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
			gameObject.layer = 12;
            mbJumping = true;
            BroadcastMessage("onPlayerJumpBegin", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void TriggerFall() {
		if (!mbJumping && !mbFalling) {
            // nop you can't fall through a bottom layered collision.
            bool bOnLowestLevel = false;
            int bottomLayer = RunnerGameManager.GetInstance().LevelManager.BottomLayer;
            List<Collider> currentColliders = mPlayerTrigger.CurrentColliders;
            foreach (Collider currentCollider in currentColliders) {
                if (currentCollider != null
                    && currentCollider.gameObject.layer == bottomLayer)
                {
                    bOnLowestLevel = true;
                    break;
                }
            }

            if (!bOnLowestLevel) {
                gameObject.layer = 12;
                mbFalling = true;
                BroadcastMessage("onPlayerFallBegin", SendMessageOptions.DontRequireReceiver);
            }
		}
	}
}
