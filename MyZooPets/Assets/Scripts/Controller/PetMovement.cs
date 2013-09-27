using UnityEngine;
using System.Collections;

public class PetMovement : Singleton<PetMovement> {
    public Camera mainCamera;
    public GameObject runWay; //Where the pet is allowed to move
    public GameObject petSprite;
	public GameObject shadowObject;
	
	// lwf animator
	public PetAnimator scriptAnim;
	
	// sound for when the pet moves
	public string strSoundMove;

    private tk2dSpriteAnimator anim; //2D sprite animator
    private Vector3 destinationPoint; //destination that the pet is going to move to
	private bool moving; //Is Pet moving now or not
	private float moveToX;
	private float moveToZ;
    private Camera nguiCamera; //Use to check if user is clicking on NGUI element. Pet shouldn't
                                //be moved when clicking on NGUI

    void Awake(){
        D.Assert(mainCamera != null, "Camera missing in " + this);
        D.Assert(petSprite != null, "PetSprite missing in " + this);
        // anim = petSprite.GetComponent<tk2dSpriteAnimator>();
        int layerNGUI = LayerMask.NameToLayer("NGUI");
        nguiCamera = NGUITools.FindCameraForLayer(layerNGUI);
    }

    void Start(){
       destinationPoint = petSprite.transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (moving && petSprite != null){
            if (ClickManager.Instance.CanRespondToTap()){ //move the pet location if allowed
                petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,
                    destinationPoint,8f * Time.deltaTime);
            }else
				StopMoving();

            //when the sprite reaches destination. stop transform and animation
            if(petSprite.transform.position == destinationPoint)
				StopMoving();
        }
    }

    //Listen to OnTap Event from FingerGesture
    void OnTap(TapGesture gesture) { 
        // if clicking is locked, ie. a GUI popup is being displayed, then don't move the pet
        if(!ClickManager.Instance.CanRespondToTap() || IsTouchingNGUI(gesture.Position)) return;
		
		AudioManager.Instance.PlayClip( strSoundMove );

        MovePet(Camera.main.ScreenPointToRay(gesture.Position));    
    }

	public void MovePetWithCamera(){
		MovePet(mainCamera.ScreenPointToRay(new Vector3(Screen.width/3, 80, 0)));
	}

    // //What to do when animation is finished playing. 
    // private void OnAnimationFinished(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip){
    //     if(moving){
    //         anim.Play("HappyWalk");
    //     }else{
    //         anim.Play("HappyIdle");
    //     }
    // }
	
	private void StopMoving() {
    	moving = false;
		scriptAnim.StopMoving();
	}
	
    //Check if the touch is in walkable area then move/animate pet
    private void MovePet(Ray myRay){
        RaycastHit hit;
        // Debug.DrawRay(myRay.origin, myRay.direction * 50, Color.green, 50f);
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == runWay.collider){
                destinationPoint = hit.point;
				
				// tell the pet animator script to tart moving
                scriptAnim.StartMoving();
				
                moving = true;
            }
        }
        ChangePetFacingDirection();
    }
	
	
	public float fShadow = .6f;
	
    //Decides when to flip sprite by comparing the screen position of the sprite and
    //the last tap screen position
    private void ChangePetFacingDirection(){
        if(destinationPoint.x > petSprite.transform.position.x){
            //petSprite.GetComponent<tk2dSprite>().FlipX = true;
			scriptAnim.Flip( true );
			shadowObject.transform.localPosition = new Vector3(fShadow, 
                shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }else{
            //petSprite.GetComponent<tk2dSprite>().FlipX = false;
			scriptAnim.Flip( false );
			shadowObject.transform.localPosition = new Vector3(-fShadow, 
                shadowObject.transform.localPosition.y, shadowObject.transform.localPosition.z);
        }
    }

    //True: if finger touches NGUI 
    private bool IsTouchingNGUI(Vector2 screenPos){
        Ray ray = nguiCamera.ScreenPointToRay (screenPos);
        RaycastHit hit;
        int layerMask = 1 << 10; 
        bool isOnNGUILayer = false;

        // Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            isOnNGUILayer = true;
        }
        return isOnNGUILayer;
    }
}
