using UnityEngine;
using System;

/// <summary>
/// Ninja trigger. The actual object that the player can cut.
/// </summary>
public class NinjaTrigger : MonoBehaviour{
	public EventHandler<EventArgs> NinjaTriggerCut;

	// sounds
	public string strSoundHit;
	public int soundHitVariations;
	public string strSoundMissed;
	public string strSoundLaunch;

	// number of children objects that are visible
	private int visibleChildrenCount;
	
	// saved velocities on this object for when it is paused/resumed
	private Vector3 savedVelocity;
	private Vector3 savedAngularVelocity;	
		
	// is this object in the process of being cut?  Necessary because we use multiple colliders on some objects
	private bool isCut = false;
	
	// particle effect that will play when this object gets hit
	public GameObject goHitFX;
	public GameObject goHitFX2;
	public GameObject goHitDirectionFX;
	public float yVal;

	protected virtual void Start(){
		yVal = transform.position.y;
		// count the number of children that have NinjaTriggerChildren scripts -- this will be used when determining if the
		// trigger is being shown by the camera or not.
		NinjaTriggerChild[] children = gameObject.GetComponentsInChildren<NinjaTriggerChild>();
		visibleChildrenCount = children.Length;
		
		// we don't want our objects colliding with each other
		GetComponent<Rigidbody>().detectCollisions = false;	
		

		AudioManager.Instance.PlayClip(strSoundLaunch, variations:3);
	}
	

	
	//---------------------------------------------------
	// OnCut()
	// When this trigger gets cut. vHit is the 2d location
	// where the trigger was precisely hit.
	//---------------------------------------------------	
	public void OnCut(Vector2 vHit){
		// if this object was already cut, return.  This is possible because some objects use multiple primitive colliders
		if(isCut){
			return;
		}
		
		// mark the object as cut
		isCut = true;
		
		// play a sound (if it exists)
		if(!string.IsNullOrEmpty(strSoundHit)){
			AudioManager.Instance.PlayClip(strSoundHit, variations:soundHitVariations);
		}
		
		// also create a little explosion particle FX where the user's finger was
		Vector3 vPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(vHit.x, vHit.y, 10));
		vPosWorld.z = goHitFX.transform.position.z;
		ParticleUtils.CreateParticle(goHitFX, vPosWorld);
		if(goHitFX2 != null){
			ParticleUtils.CreateParticle(goHitFX2, vPosWorld);
		}
		
		// Directional particle spawn
		if(goHitDirectionFX != null){
			GameObject dirParticle = ParticleUtils.CreateParticle(goHitDirectionFX, vPosWorld);
			Vector2 trailMoveDelta = NinjaGameManager.Instance.GetTrailDeltaMove();
			dirParticle.GetComponent<XYComponentRotateObject>().x = trailMoveDelta.x;
			dirParticle.GetComponent<XYComponentRotateObject>().y = trailMoveDelta.y;	
		}

		if(NinjaTriggerCut != null){
			NinjaTriggerCut(this, EventArgs.Empty);
		}
		
		// call child behaviour
		_OnCut();
	}
	
	//---------------------------------------------------
	// _OnCut()
	//---------------------------------------------------		
	protected virtual void _OnCut(){
		// children implement this	
	}
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(object sender, EventArgs args){
		// since a new game is beginning, regardless of anything, destroy ourselves
		Destroy(gameObject);
	}
	
	//---------------------------------------------------
	// OnGameStateChanged()
	// When the game's state changes, the object may
	// want to react.
	//---------------------------------------------------	
	private void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		
		switch(eState){
		case MinigameStates.GameOver:
				// pause the object
			OnPause(true);
			break;
		case MinigameStates.Paused:
				// pause the object
			OnPause(true);
			break;
		case MinigameStates.Playing:
				// unpause the object
			OnPause(false);
			break;
		}
	}	
	
	//---------------------------------------------------
	// OnPause()
	// The object will react when the game is paused or
	// unpaused.
	//---------------------------------------------------		
	private void OnPause(bool bPaused){
		if(bPaused){
			// game is pausing, so save velocities and stop movement
			savedVelocity = GetComponent<Rigidbody>().velocity;
			savedAngularVelocity = GetComponent<Rigidbody>().angularVelocity;
			GetComponent<Rigidbody>().isKinematic = true;
		}
		else{
			// game is unpausing, so resume movement and reapply velocities
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<Rigidbody>().AddForce(savedVelocity, ForceMode.VelocityChange);
			GetComponent<Rigidbody>().AddTorque(savedAngularVelocity, ForceMode.VelocityChange);			
		}
	}
	
	//---------------------------------------------------
	// ChildBecameInvis()
	// When a child of this trigger becomes invisible.
	// This isn't the best/most completely implementation
	// becaues it assumes that once a children becomes
	// invisible, it will not become visible again.  This
	// is currently the case.
	//---------------------------------------------------	
	public void ChildBecameInvis(){
		visibleChildrenCount--;
		
		// if there are no more children visible...
		if(visibleChildrenCount == 0){
			TriggerOffScreen();
		}
	}
	
	//---------------------------------------------------
	// OnBecameInvisible()
	// Nifty callback function that will tell us when
	// the trigger is no longer being rendered by the
	// camera.
	// NOTE: REQUIRES RENDERER COMPONENT!
	//---------------------------------------------------		
	void OnBecameInvisible(){
		TriggerOffScreen();
	}
	
	//---------------------------------------------------
	// TriggerOffScreen()
	// This trigger is no longer on the screen.
	//---------------------------------------------------	
	private void TriggerOffScreen(){
		// check to make sure the proper managers exist.  This check is necessary because this function will be triggered when the editor
		// quits the game, and also when the user quits the game into another scene.
		if(!AudioManager.Instance || !NinjaGameManager.Instance)
			return;

		// be absolutely sure that the game is playing...this is kind of hacky, but I was running into problems with this being called
		// despite the game being over (because the object was becoming invisible).

		if(NinjaGameManager.Instance.isGameOver)
			return;	
		
		// if the object is going invisible and was cut, just destroy it
		if(isCut)
			Destroy(gameObject);
		else{
			if(transform.position.y <= yVal+5 && !NinjaGameManager.Instance.isBouncyTime){
				//if(this.GetComponent<NinjaTriggerBomb>() ==null){
				// otherwise, it means the object was missed
					OnMissed();
				//}
			}
			else{
				if(this.GetComponent<NinjaTriggerBomb>() ==null){
					gameObject.GetComponent<Rigidbody>().AddForce(-(gameObject.GetComponent<Rigidbody>().velocity*(100)));
				}
			}
		}		
	}
	
	//---------------------------------------------------
	// OnMissed()
	// The object was not destroyed by the player.
	//---------------------------------------------------	
	private void OnMissed(){		
		// call children first
		_OnMissed();
		
		// play a sound (if it exists)
		if(!string.IsNullOrEmpty(strSoundMissed))
			AudioManager.Instance.PlayClip(strSoundMissed);		
		
		// destroy the object
		Destroy(gameObject);	
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected virtual void _OnMissed(){
		// children should implement this
	}
}
