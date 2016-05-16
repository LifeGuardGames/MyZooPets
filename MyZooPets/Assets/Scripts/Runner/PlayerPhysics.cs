using UnityEngine;
using System.Collections;

/*
    This class controls the physics of the runner player. Using a
    rigid body or character controller doesn't really give us the
    game physics that we want, so we implemented a simple physics 
    class to handle jumping, falling, and colliding    
*/

[RequireComponent (typeof(BoxCollider))]
public class PlayerPhysics : MonoBehaviour {
    public Animator anim;

    private Vector3 colliderSize;
    private Vector3 colliderCenter;
    private float skin = .005f; //Padding for collider

    private float deltaY;
    private float deltaX;
    private Vector2 pos;

    private bool grounded; //is player on the ground?
    private bool falling; //is player falling?
    private bool jumping; //is player jumpingn
    private bool movementStopped;
    private Ray ray;
    private RaycastHit hit;

    public bool AllowPassThroughLayer{get; set;}
  
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PlayerPhysics"/> is grounded.
    /// </summary>
    /// <value><c>true</c> if grounded; otherwise, <c>false</c>.</value>
    public bool Grounded{
        get{return grounded;}
        set{
            if(value != grounded){
                grounded = value;
                // print("grounded: " + grounded);
                anim.SetBool("Grounded", grounded);

                // if(grounded)
                //     BroadcastMessage("OnGrounded", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PlayerPhysics"/> is falling.
    /// </summary>
    /// <value><c>true</c> if falling; otherwise, <c>false</c>.</value>
    public bool Falling{
        get{return falling;}
        set{
            if(value != falling){
                falling = value;
                // print("falling: " + falling);
                anim.SetBool("Falling", falling);
                // if(falling)
                //     BroadcastMessage("OnFalling", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PlayerPhysics"/> is jumping.
	/// </summary>
	/// <value><c>true</c> if jumping; otherwise, <c>false</c>.</value>
    public bool Jumping{
        get{return jumping;}
        set{
            if(value != jumping){
                jumping = value;
                // print("jumping: " + jumping);
                anim.SetBool("Jumping", jumping);
                // if(jumping)
                //     BroadcastMessage("OnJumping", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PlayerPhysics"/> movement stopped.
	/// </summary>
	/// <value><c>true</c> if movement stopped; otherwise, <c>false</c>.</value>
    public bool MovementStopped{
        get{return movementStopped;}   
        set{
            if(value != movementStopped){
                movementStopped = value;
                BroadcastMessage("OnMovementStopped", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    void Start() {
        colliderSize = GetComponent<BoxCollider>().size;
        colliderCenter = GetComponent<BoxCollider>().center;
    }

    public void Reset(){
        MovementStopped = false;
        grounded = false;
        falling = false;
        jumping = false;
        AllowPassThroughLayer = false;
		MagneticField.Instance.enabled=false;
    }
	
	/// <summary>
	/// This function calls the collision dedection functions and actually move
	/// the player
	/// </summary>
	/// <param name="moveAmount">Move amount.</param>
    public void Move(Vector2 moveAmount) {
        
        deltaY = moveAmount.y;
        deltaX = moveAmount.x;
        pos = transform.position;

        //Check for collisions and modify deltaX or deltaY if necessary
        CheckCollisionsAboveAndBelow();
        CheckCollisionsLefAndRight();

        //If player is moving down and is not grounded the the playing is falling
        if(deltaY < 0 && !Grounded)
            Falling = true;


        //Move the player according to the calculated deltaX, deltaY 
        Vector2 finalTransform = new Vector2(deltaX,deltaY);
        transform.Translate(finalTransform);
    }
	
	/// <summary>
	/// Checks the collisions above and below.
	/// Currently we don't really care about the collision above so we just ignore it
	/// </summary>
    private void CheckCollisionsAboveAndBelow(){
        float dir = Mathf.Sign(deltaY);
        // Grounded = false;

        //Create 3 rays that switch between top and bottom depending on movement direction
        //detects above and below collision.
        for (int i=0; i<3; i++) {
            float x = (pos.x + colliderCenter.x - colliderSize.x/2) + colliderSize.x/2 * i; // Left, centre and then rightmost point of collider
            float y = pos.y + colliderCenter.y + colliderSize.y/2 * dir; // Bottom of collider
            ray = new Ray(new Vector2(x, y), new Vector2(0, dir));
            Debug.DrawRay(ray.origin,ray.direction);

            if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaY))){
                // Get Distance between player and ground
                float dst = Vector3.Distance(ray.origin, hit.point);

                //We only care about the colliding physic for the bottom rays
                if(dir == -1){
                    //If the layer can be pass through than break and ignore collision
                    int hitLayer = hit.collider.transform.gameObject.layer;
                    if(hitLayer == LayerMask.NameToLayer("PassThroughLayer") &&
                        AllowPassThroughLayer){
                        AllowPassThroughLayer = false;
                        break;
                    }

                    //Items on ItemsLayer handle their own collision, so ignore collision here
                    if(hitLayer == LayerMask.NameToLayer("ItemsLayer"))
                        break;

                    AllowPassThroughLayer = false;

                    // Stop player's downwards movement after coming within skin width of a collider
                    if(dst > skin){
                        deltaY = dst * dir - skin * dir;
                    }else{
                        deltaY = 0;
                    }

                    //Player hits the collider, so grounded is true and everything else is false
                    Jumping = false;
                    Falling = false;
                    Grounded = true;
                }

                break; //Break when collision is detected
            }else
                //If the raycast doesn't hit anything than the player is not grounded
                Grounded = false;
        }
    }

	/// <summary>
	/// Checks the collisions lef and right.
	/// </summary>
    private void CheckCollisionsLefAndRight(){
        float dir = Mathf.Sign(deltaX);
        // MovementStopped = false;

        //Creates 3 rays that switches left or right depending on the movement.
        //This 3 rays are used to detect left/right collisions
        for(int i=0; i<3; i++){
            float x = pos.x + colliderCenter.x + colliderSize.x/2 * dir;
            float y = pos.y + colliderCenter.y - colliderSize.y/2 + colliderSize.y/2;

            ray = new Ray(new Vector2(x, y), new Vector2(dir, 0));
            Debug.DrawRay(ray.origin,ray.direction);
            
            if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaX) + skin)) {
                // Get Distance between player and ground
                float dst = Vector3.Distance (ray.origin, hit.point);

                //Don't allow collision for pass through layer or items layer
                //Items on ItemsLayer will handle their own collision
                int hitLayer = hit.collider.transform.gameObject.layer;
                if(hitLayer == LayerMask.NameToLayer("PassThroughLayer") ||
                    hitLayer == LayerMask.NameToLayer("ItemsLayer"))
                    break;

                // Stop player's movement after coming within skin width of a collider
                if (dst > skin)
                    deltaX = dst * dir - skin * dir;
                else
                    deltaX = 0;
                
                MovementStopped = true;
                break; //Break when collision is detected
            }
        }
    }
}
