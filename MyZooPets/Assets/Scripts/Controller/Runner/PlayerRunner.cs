using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerRunner : MonoBehaviour
{
	public float Speed = 0.1f;
	public float JumpSpeed = 0.5f;
    public float Mass = 1.0f;
	
	private bool mbColliding = false;
	private bool mbJumping = false;
    private bool mbGrounded = false;
    private Vector3 mMovementVector = Vector3.zero;
    private CharacterController mCharacterController;
	
	// Use this for initialization
	void Start ()
	{
        mCharacterController = gameObject.GetComponent<CharacterController>();
        if (mCharacterController == null)
            Debug.LogError("Character Controller not attached!");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (mCharacterController == null)
            mCharacterController = GetComponent<CharacterController>();
            
		//UpdateInput();
		UpdateMovement();
	}
	
	void FixedUpdate()
	{
	}
	
	void UpdateMovement()
	{
	    if(mbGrounded)
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
	
	void OnCollisionEnter()
	{
		mbColliding = true;
		mbJumping = false;
	}

	void OnCollisionExit()
	{
		mbColliding = false;
	}
}
