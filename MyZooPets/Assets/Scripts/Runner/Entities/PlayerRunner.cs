/* Sean Duane
 * PlayerRunner.cs
 * 8:26:2013   12:26
 * Description:
 * This script is what controls everything the player does in the runner game.
 * It's arguably the largets and heaviest script.
 * 
 * INPUT:
 * Input is pulled from the InputManager, which triggers certain events here. Like onSwipeUp().
 * 
 * MOVEMENT:
 * The character uses a charactercontroller. So, we also use the .Move() method of that controller.
 * The movement is done with psuedo-forces.
 * A "movementvector" keeps track of certain forces, like jumping.
 * Then we apply the different forces to this vector. Right now, we just directly set the X and Y components, since that's all we move
 * that stuff at. if you wanted to apply others, you would "add" in the vectors instead.
 * The forces are then all multiplied by dt a few times. See the f=ma equation and it's unit of measurements to understand why. tldr newton.
 * The jumping is a bit more finicky though. You just apply an upward force of the jump speed once, and it goes.
 * However, falling kinda stinks. Falling is gravity*dt. What about mass? air drag?
 * I did my best to implement those features, but they might be wrong. The mass should make the player fall more (with more mass), and less
 * drag should make it float a bit more.
 * 
 * COLLISION:
 * ..is pretty messed up. Good luck fixing this, becuase I just could not do it as much as I tried.
 * Basically:
 * The player runs around with .Move() of the CC (character controller) and a rigidbody. Those magic unity classes will let you do
 * the 'sliding' on the screen that you see.
 * However, then you get to the Jumping and Falling. How do you jump THROUGH a platform and fall THROUGH one?
 * CHANGE THIS WHEN YOU FIX COLLISION:
 * Basically, right now, it's like this- All platforms are actually one-sided mesh colliders. That means that collision sort of occurs only on one side.
 * So that takes care of jumping- it jumps through the non collision side.
 * Falling will simply raycast down, find the current thing it's standing on, check if that is NOT a ground peice (marked by its layer), then ignore collision with that.
 * When we become grounded again, we un-ignore those collisions.
 * 
 * SPEED:
 * The player has a set speed. It never changes. The timestep of the game is what makes things 'faster'. This may change from the time of this writing.
 * 
 * INVINCIBILITY:
 * Right now, it sets a flag and a time pulse. Whjeneve ryou hit a bad item, nothing happens.
 * When you die, it tries to find the topmost componetn and sticks you there, if you arent above one already.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerRunner : Singleton<PlayerRunner>{
	//    public float DefaultSpeed = 1.0f;
	// public float JumpSpeed = 0.5f;
	// public float SpeedIncrease = 0.1f;
	// public float SpeedIncreaseTime = 5;

	//    // Various pulses.
	//    // fyi, pulses are 'countdown' timers. They just get a time (in seconds for unity) shoved in them, then
	//    //subtract dt (deltaTime) from it every update. <= zero? Time has elapsed!
	//    // Also, I divide by the timescale. Cuz we want "REAL" time sconds, not "GAME" time seconds, which is what timescale is doing.
	// private float mInvinciblePulse = 0f;
	// private float mSpeedBoostPulse = 0f; // Boosts from items
	// private float mSpeedIncreasePulse = 0f; // Time til we speed up our constant speed
	//    private float mCurrentTimeMultiplier;
	// private float mSpeedBoostAmmount = 0f;
	//    private bool mbInvincible;
	//    private bool mbJumping;
	//    private bool mbGrounded;
	//    private bool mbFalling;
	//    private bool mbTriggerColliding;
	//    private Vector2 mInitialPosition;
	//    private Vector3 mMovementVector;
	//    private Vector3 mLastPosition;
	// private CharacterController mCharacterController;
	//    private List<Collider> mIgnoringCollisions = new List<Collider>();
	// private bool mbDied;

	//    public bool Invincible { get { return mbInvincible; } }

	// // Use this for initialization
	// void Start() {
	//        mInitialPosition = transform.position;

	//        mCharacterController = gameObject.GetComponent<CharacterController>();
	//        if (mCharacterController == null)
	//            Debug.LogError("Character Controller not attached!");

	//        // Slightly redundant in some ways, but keeps some logic together.
	//        Reset();
	// }

	// // Update is called once per frame
	// void Update() {
	// 	if ( !RunnerGameManager.Instance.GameRunning )
	// 		return;
		
	//        // Poll the input. Generally the InputManager handles this, so its really just for debugging.
	//        UpdateInput();
	//        // If we are invincible, update the timer.
	//        // UpdateInvincible();
	//        // If the speed timer elapses, increase game time speed.
	//        UpdateSpeed();
	//        // Some falling logic to help collision.
	//        UpdateFalling();
	// }
	
	//    // The more physics-y update. Called multiple times per frame.
	// void FixedUpdate() {
	// 	if ( !RunnerGameManager.Instance.GameRunning )
	// 		return;
		
	//        // This is what actually moves the player.
	// 	UpdateMovement();

	//        // Needs to be different, so that we can determine a vector.
	// 	if (!Vector3.Equals(mLastPosition, transform.position))
	// 		mLastPosition = transform.position;

	//        // Make sure we aint dead now that we've moved.
	// 	// CheckAndActOnDeath();
	// }

	// void onTap() {
	// 	TriggerJump();
	// }

	// void onSwipeDown() {
	// 	TriggerFall();
	// }

	//    void onPlayerJumpBegin() { }
	//    void onPlayerJumpEnd() { }
	//    void onPlayerFallBegin() { }
	//    void onPlayerFallEnd() { }
	//    void onPlayerGrounded() { }
	//    void onPlayerFreeFall() { }

	//    public void Reset() {
	//        mCurrentTimeMultiplier = 1f;
	//        // Go back to the original position.
	// 	transform.position = mInitialPosition;
	//        // Reset some timers
	//        mSpeedIncreasePulse = SpeedIncreaseTime;
	//        mSpeedBoostPulse = 0f;
	//        mInvinciblePulse = 0f;
	//        // Player values
	//        mLastPosition = transform.position;
	//        mMovementVector = Vector3.zero;
	//        mLastPosition = Vector3.zero;
	//        mbInvincible = false;
	//        // Collision values
	//        mbTriggerColliding = false;
	//        mbGrounded = false;
	//        mbFalling = false;
	//        mbJumping = false;
	// 	mbDied = false;
	//        //@TODO you may have to reset all ignoring collisions before clearing. idk.
	//        mIgnoringCollisions.Clear();

	//        RunnerAnimationController animationController = GetComponent<RunnerAnimationController>();
	//        if (animationController != null)
	//            animationController.Reset();
	//    }

	//    private void UpdateMovement() {
	//        // These are constant speeds, not forces. But I treat them like forces. It's weird I know.
	//        mMovementVector.x = DefaultSpeed + mSpeedBoostAmmount;

	//        // Add in Gravity force.
	//        mMovementVector += (Physics.gravity * rigidbody.mass) * Time.deltaTime;

	//        // Vertical drag
	//        mMovementVector += (Vector3.down * rigidbody.drag * Time.deltaTime);

	//        if (mCharacterController == null)
	//            Debug.LogError("No Character Controller exists!");

	//        // Perform the move
	//        CollisionFlags flags = mCharacterController.Move(mMovementVector * Time.deltaTime);
	//        bool isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;

	//        // Now update some states based on our groundedness
	//        if (isGrounded && mbJumping) {
	//            mbJumping = false;
	//            BroadcastMessage("onPlayerJumpEnd", SendMessageOptions.DontRequireReceiver);
	//        }

	//        if (isGrounded) {
	//            // Reset movement.
	//            mMovementVector = new Vector3();

	//            if (!mbGrounded) {
	//                // Was previously in freefall.
	//                for (int ignoredIndex = mIgnoringCollisions.Count - 1; ignoredIndex >= 0; ignoredIndex--) {
	//                    Collider ignoredCollider = mIgnoringCollisions[ignoredIndex];
	//                    if (ignoredCollider != null)
	//                        Physics.IgnoreCollision(ignoredCollider, collider, false);
	//                    else
	//                        mIgnoringCollisions.RemoveAt(ignoredIndex);
	//                }

	//                BroadcastMessage("onPlayerGrounded", SendMessageOptions.DontRequireReceiver);
	//            }
	//        } else {
	//            if (mbGrounded) {
	//                // Just entered freefall
	//                if (!mbJumping && !mbFalling) {
	//                    BroadcastMessage("onPlayerFreeFall", SendMessageOptions.DontRequireReceiver);
	// 			}
	//            }
	//        }

	//        mbGrounded = isGrounded;

	//        Vector3 position = transform.position;
	//        position.x = mInitialPosition.x;
	//        transform.position = position;
	//    }

	//    private void UpdateSpeed() {
	//        mSpeedIncreasePulse -= Time.deltaTime / Time.timeScale;
	//        if (mSpeedIncreasePulse <= 0) {
	//            //mSpeed += SpeedIncrease;
	//            mCurrentTimeMultiplier += SpeedIncrease;
	//            RunnerGameManager.Instance.IncreaseTimeSpeed(SpeedIncrease);
	//            mSpeedIncreasePulse = SpeedIncreaseTime;
	//        }

	//        if (mSpeedBoostPulse > 0f) {
	//            mSpeedBoostPulse -= Time.deltaTime / Time.timeScale;
	//            if (mSpeedBoostPulse <= 0f) {
	//                mSpeedBoostPulse = 0f;
	//                mSpeedBoostAmmount = 0f;
	//            }
	//        }
	//    }

	//    private void UpdateInvincible() {
	//        if (mbInvincible) {
	//            mInvinciblePulse -= Time.deltaTime / Time.timeScale;
	//            if (mInvinciblePulse <= 0f) {
	//                // Ready to uninvincible, except, we need to make sure we don't do it
	//                //while the character is dying...
	//                if (mbGrounded)
	//                    mbInvincible = false;
	//                else {
	//                    // Raycast around up. If there is something to latch on to, go to it!
	//                    RaycastHit hitInfo;
	//                    bool bSomethingAbove = Physics.Raycast(transform.position, Vector3.up, out hitInfo);
	//                    if (bSomethingAbove) {
	//                        // Latch onto it
	//                        Vector3 newPosition = hitInfo.transform.position;
	//                        newPosition.y += mCharacterController.height / 2;
	//                        transform.position = newPosition;

	//                        // Reset movement vector
	//                        mMovementVector = Vector3.zero;
	//                    } else {
	//                        bool bSomethingBelow = Physics.Raycast(transform.position, Vector3.down, out hitInfo);
	//                        if (bSomethingBelow)
	//                            mbInvincible = false;
	//                    }
	//                }
	//            }
	//        }
	//    }

	//    // Checks if we are "falling down" to re-eneable collision.
	//    private void UpdateFalling(Collider inCollider = null) {
	//        if (mbFalling) {
	//            if (!mbTriggerColliding || mbGrounded) {
	//                mbFalling = false;
	//                BroadcastMessage("onPlayerFallEnd", SendMessageOptions.DontRequireReceiver);
	//            }
	//        }
	//    }

	//    private void UpdateInput() {
	//        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
	//            // Add in jump, since we are grounded, if its pressed.
	//            TriggerJump();
	//        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
	//            // Add in jump, since we are grounded, if its pressed.
	//            TriggerFall();
	//        }
	//    }

	//    public void TriggerInvincibility(float inDuration) {
	//        mInvinciblePulse = inDuration;
	//        mbInvincible = true;
	//    }

	//    public void TriggerSpeedBoost(float inDuration, float inSpeedAmmount) {
	//        mSpeedBoostAmmount = inSpeedAmmount;
	//        mSpeedBoostPulse = inDuration;
	//    }

	// public void TriggerSlowdown(float inDivisor) {
	//        if(!mbInvincible){
	//            RunnerGameManager.Instance.SlowTimeSpeed(inDivisor);
	//            MegaHazard.Instance.TriggerPlayerSlowdown();
	//        }
	// }

	// private void TriggerJump() {
	// 	if (!mbJumping) {
	// 		mMovementVector.y += JumpSpeed;
	//            mbJumping = true;
	//            BroadcastMessage("onPlayerJumpBegin", SendMessageOptions.DontRequireReceiver);
			
	// 		// play jump sound
	// 		AudioManager.Instance.PlayClip( "runnerJumpUp" );
	// 	}
	// }

	// private void TriggerFall() {
	// 	if (!mbJumping && !mbFalling) {
	//            // nop you can't fall through a bottom layered collision.
	//            int bottomLayer = LevelManager.Instance.BottomLayer;

	//            RaycastHit hitinfo;
	//            if (Physics.Raycast(collider.bounds.min, Vector3.down, out hitinfo, 1f)) {
	//                Collider hitCollider = hitinfo.collider;
	//                if (hitCollider.gameObject.layer != bottomLayer) {
	//                    Physics.IgnoreCollision(hitinfo.collider, collider);
	//                    mIgnoringCollisions.Add(hitinfo.collider);
	//                    mbFalling = true;
	//                    mbGrounded = false;
	//            		BroadcastMessage("onPlayerFallBegin", SendMessageOptions.DontRequireReceiver);
					
	// 				// play jump down sound
	// 				AudioManager.Instance.PlayClip( "runnerJumpDown" );
	//                }
	//            }
	//        }
	// }

	//    private void CheckAndActOnDeath() {
	//        // Are we below the maps floor value
	//        LevelManager levelManager = LevelManager.Instance;
	//        if(transform.position.y < levelManager.LevelTooLowYValueGameOver){
	//            AudioManager.Instance.PlayClip("runnerDie");
	//            RunnerGameManager.Instance.ActivateGameOver();
	//        }
	//   //      float yTooLowValue = levelManager.GetTooLowYValue(transform.position);
	//   //      if (transform.position.y < yTooLowValue) {
	//   //          if (!mbInvincible) {
				
	// 		// 	if ( transform.position.y < levelManager.LevelTooLowYValueGameOver )
	//   //              	RunnerGameManager.Instance.ActivateGameOver();
	// 		// 	else {
	// 		// 		// play die sound (once)
	// 		// 		if ( !mbDied ) {
	// 		// 			mbDied = true;
	// 		// 			AudioManager.Instance.PlayClip( "runnerDie" );
	// 		// 		}
	// 		// 	}
	// 		// }
	//   //          else {
	//   //              Vector3 position = transform.position;
	//   //              position.y = yTooLowValue;
	//   //              transform.position = position;
	//   //          }
	//   //      }
	//    }
}
