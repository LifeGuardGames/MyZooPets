using UnityEngine;
using System.Collections;

//---------------------------------------------------
// PetAnimator
// Script that controls all the pet's animations.
//---------------------------------------------------

public class PetAnimator : LgCharacterAnimator {
	
	// key of the pet's "species" -- i.e. what kind of pet it is
	// this will eventually be set in save data probably
	public string strKeySpecies;
	public string GetSpeciesKey() {
		return strKeySpecies;	
	}
	
	// key of the pet's color
	// this will eventually be set in save data probably
	public string strKeyColor;
	public string GetColorKey() {
		return strKeyColor;	
	}
	
	// number of idles
	public int nIdleCount;
	
	private string strAnimQueued;
	
	// pet's animation state
	private PetAnimStates eAnimState;
	private void SetAnimState( PetAnimStates eState ) {
		eAnimState = eState;	
	}
	public PetAnimStates GetAnimState() {
		return eAnimState;
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
		// set the LWFAnimator loading data based on the pet's attributes
		string strSpecies = GetSpeciesKey();
		string strColor = GetColorKey();
		animName = strSpecies + strColor;
		folderPath = "LWF/" + animName + "/";
		
		// only call this AFTER we have set our loading data
		base.Start();	
		
		// the animator starts off empty, so immediately pick the animation we want to play (idle)
		Idle();
	}

	//---------------------------------------------------
	// Idle()
	//---------------------------------------------------
	private void Idle() {
		// set pet's anim state
		SetAnimState( PetAnimStates.Idling );
		
		// pick a random idle
		int nIdle = UnityEngine.Random.Range(1,nIdleCount+1);
		
		// queue the anim
		strAnimQueued = "happyIdle_" + nIdle;
	}
	
	//---------------------------------------------------
	// StartMoving()
	//---------------------------------------------------		
	public void StartMoving() {
		SetAnimState( PetAnimStates.Walking );
		PlayClip( "happyWalk" );
	}
	
	//---------------------------------------------------
	// StopMoving()
	//---------------------------------------------------		
	public void StopMoving() {
		Idle();
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------		
	void Update() {
		// call base update so that LWF can do it's thing
		base.Update();
		
		// if we have a queued animation, play it
		if ( !string.IsNullOrEmpty(strAnimQueued) )	{
			//Debug.Log("Playing queued anim: " + strAnimQueued);
			PlayClip( strAnimQueued );
			strAnimQueued = null;
		}
	}
	
	//---------------------------------------------------
	// ClipFinished()
	//---------------------------------------------------		
	protected override void ClipFinished() {
		PetAnimStates eState = GetAnimState();
		
		switch ( eState ) {
			case PetAnimStates.Idling:
				Idle();
				break;				
			case PetAnimStates.Walking:
				// do nothing; the anim will loop
				break;
			default:
				Debug.Log("Unhandled pet anim state finishing: " + eState);
				Idle();
				break;
		}
	}
}
