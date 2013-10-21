using UnityEngine;
using System.Collections;


[RequireComponent (typeof(BoxCollider))]
public class PlayerPhysics : MonoBehaviour {
    private Vector3 colliderSize;
    private Vector3 colliderCenter;
    private float skin = .005f; //Padding for collider

    private float deltaY;
    private float deltaX;
    private Vector2 pos;
    
    public bool Grounded{get; set;} //when player touching the ground or falling
    public bool Jumping{get; set;} //when player is jumping up. In the upward trajectory. 
    public bool MovementStopped{get; set;}
    
    Ray ray;
    RaycastHit hit;
    
    void Start() {
        colliderSize = GetComponent<BoxCollider>().size;
        colliderCenter = GetComponent<BoxCollider>().center;
    }

    public void Move(Vector2 moveAmount, ref int numOfLayerToPassThrough) {
        
        deltaY = moveAmount.y;
        deltaX = moveAmount.x;
        pos = transform.position;

        CheckCollisionsAboveAndBelow(ref numOfLayerToPassThrough);
        CheckCollisionsLefAndRight();
        
        Vector2 finalTransform = new Vector2(deltaX,deltaY);
        transform.Translate(finalTransform);
    }

    private void CheckCollisionsAboveAndBelow(ref int numOfLayerToPassThrough){
        Grounded = false;

        //Create 3 rays that switch between top and bottom depending on movement direction
        //detects above and below collision.
        for (int i=0; i<3; i++) {
            float dir = Mathf.Sign(deltaY);
            float x = (pos.x + colliderCenter.x - colliderSize.x/2) + colliderSize.x/2 * i; // Left, centre and then rightmost point of collider
            float y = pos.y + colliderCenter.y + colliderSize.y/2 * dir; // Bottom of collider
            ray = new Ray(new Vector2(x, y), new Vector2(0, dir));
            Debug.DrawRay(ray.origin,ray.direction);

            if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaY))){
                // Get Distance between player and ground
                float dst = Vector3.Distance(ray.origin, hit.point);

                //when user is falling
                if(dir == -1){
                    //If the layer can be pass through than break and ignore collision
                    if(hit.collider.transform.gameObject.layer == LayerMask.NameToLayer("PassThroughLayer") &&
                        numOfLayerToPassThrough == 1){
                        numOfLayerToPassThrough--;
                        break;
                    }

                    // Stop player's downwards movement after coming within skin width of a collider
                    if(dst > skin)
                        deltaY = dst * dir - skin * dir;
                    else
                        deltaY = 0;

                    Grounded = true;
                    Jumping = false;
                }else{
                    Jumping = true;
                }
                break; //Break when collision is detected
            }
        }
    }

    private void CheckCollisionsLefAndRight(){
        MovementStopped = false;

        //Creates 3 rays that switches left or right depending on the movement.
        //This 3 rays are used to detect left/right collisions
        // for(int i=0; i<3; i++){
            float dir = Mathf.Sign(deltaX);
            float x = pos.x + colliderCenter.x + colliderSize.x/2 * dir;
            float y = pos.y + colliderCenter.y - colliderSize.y/2 + colliderSize.y/2;

            ray = new Ray(new Vector2(x, y), new Vector2(dir, 0));
            Debug.DrawRay(ray.origin,ray.direction);
            
            if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaX) + skin)) {
                // Get Distance between player and ground
                float dst = Vector3.Distance (ray.origin, hit.point);

                //Don't allow collision for pass through layer or items layer
                int hitLayer = hit.collider.transform.gameObject.layer;
                if(hitLayer == LayerMask.NameToLayer("PassThroughLayer") ||
                    hitLayer == LayerMask.NameToLayer("ItemsLayer"))
                    return;

                // Stop player's downwards movement after coming within skin width of a collider
                if (dst > skin)
                    deltaX = dst * dir - skin * dir;
                else
                    deltaX = 0;
                
                MovementStopped = true;
                // break; //Break when collision is detected
            }
        // }
    }
}
